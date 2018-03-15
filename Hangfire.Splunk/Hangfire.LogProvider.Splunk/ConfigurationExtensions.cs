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
        /// <param name="globalConfiguration">Instance of <see cref="IGlobalConfiguration"/>.</param>
        /// <param name="splunkConfiguration">Instance of <see cref="IConfigurationProvider"/>.</param>
        public static IGlobalConfiguration UseSplunkLogProvider(this IGlobalConfiguration globalConfiguration, IConfigurationProvider splunkConfiguration)
        {
            if (splunkConfiguration == null)
                return globalConfiguration;

            return globalConfiguration.UseLogProvider(new SplunkLogProvider(splunkConfiguration));
        }
    }
}