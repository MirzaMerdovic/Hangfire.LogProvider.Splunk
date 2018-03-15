using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Logging;
using Hangfire.LogProvider.Splunk.Configuration;
using Newtonsoft.Json;

namespace Hangfire.LogProvider.Splunk
{
    /// <summary>
    /// Represents the implementation of <see cref="T:Hangfire.Logging.ILogProvider" /> that uses Splunk.
    /// </summary>
    public class SplunkLogProvider : ILogProvider
    {
        private static ConcurrentDictionary<string, ILog> _loggers = new ConcurrentDictionary<string, ILog>();
        private static HttpClient _client = new HttpClient();

        private readonly IConfigurationProvider _configuration;

        /// <summary>
        /// Initializes new instance of <see cref="SplunkLogProvider"/>.
        /// </summary>
        /// <param name="configuration">Instance of <see cref="IConfigurationProvider"/>.</param>
        public SplunkLogProvider(IConfigurationProvider configuration)
        {
            _configuration = configuration ?? throw new NotImplementedException(nameof(configuration));

            if (_client.BaseAddress == null)
                _client.BaseAddress = configuration.BaseUrl;

            if (_client.DefaultRequestHeaders.Authorization == null)
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Splunk", configuration.Token);
        }

        /// <summary>
        /// Resolves <see cref="ILog"/> instance.
        /// </summary>
        /// <param name="name">Splunk's default field that identifies the source of an event, that is, where the event originated.</param>
        /// <remarks>
        /// More info: https://docs.splunk.com/Splexicon:Source
        /// If not provided value will be the name of executing assembly.
        /// </remarks>
        /// <returns>Instance of <see cref="T:Hangfire.Logging.ILog"/>.</returns>
        public ILog GetLogger(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = Assembly.GetExecutingAssembly().GetName().Name;

            if (_loggers.ContainsKey(name))
                return _loggers[name];

            _loggers[name] = new SplunkLogger(name, _configuration);

            return _loggers[name];
        }

        /// <summary>
        /// Tries to send all events from bucket to Splunk ignoring the configured bucket size limit.
        /// </summary>
        public static async Task Flush()
        {
            await SplunkLogger.SendToSplunk(_client).ConfigureAwait(false);
        }

        /// <summary>
        /// Represents implementation of <see cref="T:Hangfire.Logging.ILog"/> interface that provides mean.
        /// </summary>
        internal class SplunkLogger : ILog
        {
            private static ConcurrentQueue<string> _bucket;
            private readonly string _source;

            private readonly IConfigurationProvider _configuration;

            /// <summary>
            /// Initializes new instance of <see cref="SplunkLogger"/>.
            /// </summary>
            /// <param name="source">Splunk source field.</param>
            /// <param name="configuration">Instance of <see cref="IConfigurationProvider"/>.</param>
            public SplunkLogger(string source, IConfigurationProvider configuration)
            {
                _source = source;
                _configuration = configuration;

                _bucket = new ConcurrentQueue<string>();
            }

            /// <summary>
            /// <see cref="T:Hangfire.Logging.ILog.Log"/>
            /// </summary>
            public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
            {
                if (messageFunc == null)
                    return true;

                if (!Enum.TryParse(_configuration.MinimumLogLevel, out LogLevel minimumLevel))
                    minimumLevel = LogLevel.Warn;

                if (_configuration.AllowDynamicLogLevelSwitch)
                {
                    if (Enum.TryParse(_configuration.CheckMinimumLogLevel(), out LogLevel level))
                        minimumLevel = level;
                }

                if (logLevel < minimumLevel)
                    return false;

                if (_configuration.BucketSize > _bucket.Count)
                    _bucket.Enqueue(JsonConvert.SerializeObject(CreateMessage(messageFunc(), exception)));

                if (_configuration.BucketSize != _bucket.Count)
                    return true;

                return SendToSplunk(_client).GetAwaiter().GetResult();

                object CreateMessage(string message, Exception ex)
                {
                    if (string.IsNullOrWhiteSpace(message))
                        throw new ArgumentNullException($"{nameof(message)} --Thread: {Thread.CurrentThread.ManagedThreadId}");

                    return new
                    {
                        source = _source,
                        sourcetype = _configuration.SourceType,
                        index = _configuration.Index,
                        @event = new
                        {
                            level = logLevel.ToString(),
                            message,
                            exception = ex
                        }
                    };
                }
            }

            internal static async Task<bool> SendToSplunk(HttpClient client)
            {
                StringContent content = null;

                try
                {
                    var temp = CombineEventContent();

                    if(string.IsNullOrEmpty(temp))
                        return false;

                    content = new StringContent(temp, Encoding.UTF8, "application/json");
                    using (var request = new HttpRequestMessage(HttpMethod.Post, "/services/collector/event"))
                    {
                        request.Content = content;
                        content = null;

                        var response = await client.SendAsync(request).ConfigureAwait(false);

                        return response.IsSuccessStatusCode;
                    }

                    string CombineEventContent()
                    {
                        var events = string.Empty;

                        while (_bucket.Count > 0)
                        {
                            if (_bucket.TryDequeue(out var e))
                                events += e;
                        }

                        return events;
                    }
                }
                finally
                {
                    content?.Dispose();
                }
            }
        }
    }
}