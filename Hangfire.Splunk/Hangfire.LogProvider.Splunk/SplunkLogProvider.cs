using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        private static HttpClient _client = new HttpClient();

        private readonly SplunkLogProviderSection _configuration;

        /// <summary>
        /// Initializes new instance of <see cref="SplunkLogProvider"/>.
        /// </summary>
        /// <param name="configuration">Instance of <see cref="SplunkLogProviderSection"/>.</param>
        public SplunkLogProvider(SplunkLogProviderSection configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _client = ConfigureClient(_configuration);
        }

        /// <summary>
        /// Tries to send all events from bucket to Splunk ignoring the configured bucket size limit.
        /// </summary>
        public static async Task Flush()
        {
            await SplunkLogger.SendToSplunk().ConfigureAwait(false);
        }

        /// <summary>
        /// Resolves <see cref="ILog"/> instance.
        /// </summary>
        /// <param name="name">Not required.</param>
        /// <returns>Instance of <see cref="T:Hangfire.Logging.ILog"/>.</returns>
        public ILog GetLogger(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                _configuration.Source = name;

            return new SplunkLogger(_configuration);
        }

        private static HttpClient ConfigureClient(SplunkLogProviderSection configuration)
        {
            _client.BaseAddress = new Uri(configuration.BaseUrl);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Splunk", configuration.Token);

            return _client;
        }

        /// <summary>
        /// Represents implementation of <see cref="T:Hangfire.Logging.ILog"/> interface that provides mean.
        /// </summary>
        internal class SplunkLogger : ILog
        {
            private static readonly ConcurrentQueue<string> Bucket = new ConcurrentQueue<string>();

            private readonly SplunkLogProviderSection _configuration;

            /// <summary>
            /// Initializes new instance of <see cref="SplunkLogger"/>.
            /// </summary>
            public SplunkLogger(SplunkLogProviderSection configuration)
            {
                _configuration = configuration;
            }

            /// <summary>
            /// <see cref="T:Hangfire.Logging.ILog.Log"/>
            /// </summary>
            public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
            {
                if (messageFunc == null)
                    return true;

                if (logLevel < _configuration.LoggingLevel)
                    return false;

                var message = 
                    CreateMessage(
                        _configuration.Source, 
                        _configuration.SourceType, 
                        _configuration.Index, 
                        logLevel.ToString(), 
                        messageFunc(), 
                        exception);

                if (_configuration.BucketSize > Bucket.Count)
                    Bucket.Enqueue(JsonConvert.SerializeObject(message));

                if (_configuration.BucketSize != Bucket.Count)
                    return true;

                return SendToSplunk().GetAwaiter().GetResult();
            }

            internal static async Task<bool> SendToSplunk()
            {
                StringContent content = null;

                try
                {
                    content = new StringContent(CombineEventContent(), Encoding.UTF8, "application/json");
                    using (var request = new HttpRequestMessage(HttpMethod.Post, "/services/collector/event"))
                    {
                        request.Content = content;
                        content = null;

                        var response = await _client.SendAsync(request).ConfigureAwait(false);

                        return response.IsSuccessStatusCode;
                    }
                }
                finally
                {
                    content?.Dispose();
                }
            }

            private static string CombineEventContent()
            {
                var events = string.Empty;

                while (Bucket.Count > 0)
                {
                    Bucket.TryDequeue(out var e);
                    events += e;
                }

                return events;
            }

            private static object CreateMessage(string source, string sourceType, string index, string level, string message, Exception exception)
            {
                return new
                {
                    source,
                    sourcetype = sourceType,
                    index,
                    @event = new
                    {
                        level,
                        message,
                        exception
                    }
                };
            }
        }
    }
}