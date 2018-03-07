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
        public static IGlobalConfiguration UseSplunkLogProvider(this IGlobalConfiguration configuration)
        {
            var splunkConfig = TryGetSplungLogProviderSection();

            if (splunkConfig == null)
                return configuration;

            if (!Uri.IsWellFormedUriString(splunkConfig.BaseUrl, UriKind.Absolute))
                return configuration;

            return 
                string.IsNullOrWhiteSpace(splunkConfig.Token) 
                    ? configuration 
                    : configuration.UseLogProvider(new SplunkLogProvider(splunkConfig));
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