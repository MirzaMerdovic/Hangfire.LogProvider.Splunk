using System;
using System.Configuration;
using Hangfire.LogProvider.Splunk.Configuration;

namespace Hangfire.LogProvider.Splunk
{
    /// <summary>
    /// Extension method that enables Splunk logging for Hangfire.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Tries to set up Splunk logging configuration.
        /// </summary>
        /// <param name="configuration">Instance of <see cref="IGlobalConfiguration"/>.</param>
        /// <param name="disposable">SplunkLogProvider instance contains HttpClient so it must be disposed.</param>
        public static IGlobalConfiguration UseSplunkLogProvider(this IGlobalConfiguration configuration, out IDisposable disposable)
        {
            disposable = null;
            var splunkConfig = TryGetSplungLogProviderSection();

            if (splunkConfig == null)
                return configuration;

            if (!Uri.IsWellFormedUriString(splunkConfig.BaseUrl, UriKind.Absolute))
                return configuration;

            if (string.IsNullOrWhiteSpace(splunkConfig.Token))
                return configuration;

            var provider = new SplunkLogProvider(splunkConfig);
            disposable = provider;

            return configuration.UseLogProvider(provider);
        }

        private static SplunkLogProviderSection TryGetSplungLogProviderSection()
        {
            try
            {
                return (SplunkLogProviderSection)ConfigurationManager.GetSection("SplunkLogProvider");
            }
            catch
            {
                return null;
            }
        }
    }
}