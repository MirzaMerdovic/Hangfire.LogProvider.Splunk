using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

            _client = InitializeClient(_configuration);
        }

        /// <summary>
        /// Resolves <see cref="ILog"/> instance.
        /// </summary>
        /// <param name="name">Not required.</param>
        /// <returns>Instance of <see cref="ILog"/>.</returns>
        public ILog GetLogger(string name)
        {
            return new SplunkLogger(_configuration.LoggingLevel);
        }

        private static HttpClient InitializeClient(SplunkLogProviderSection configuration)
        {
            _client.BaseAddress = new Uri(configuration.BaseUrl);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Splunk", configuration.Token);

            return _client;
        }

        internal class SplunkLogger : ILog
        {
            private readonly LogLevel _minimumLevel;

            public SplunkLogger(LogLevel minimumLevel)
            {
                _minimumLevel = minimumLevel;
            }

            public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
            {
                if (messageFunc == null)
                    return true;

                if (logLevel < _minimumLevel)
                    return false;

                var message = CreateMessage(logLevel.ToString(), messageFunc(), exception);

                StringContent content = null;
                try
                {
                    content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                    using (var request = new HttpRequestMessage(HttpMethod.Post, "/services/collector/event"))
                    {
                        request.Content = content;
                        content = null;

                        var response = _client.SendAsync(request).GetAwaiter().GetResult();

                        return response.IsSuccessStatusCode;
                    }
                }
                finally
                {
                    content?.Dispose();
                }
            }

            private static object CreateMessage(string level, string message, Exception exception)
            {
                return new
                {
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