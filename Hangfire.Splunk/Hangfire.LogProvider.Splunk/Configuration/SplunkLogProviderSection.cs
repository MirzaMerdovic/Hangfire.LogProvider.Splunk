using System.Configuration;
using Hangfire.Logging;

namespace Hangfire.LogProvider.Splunk.Configuration
{
    /// <inheritdoc />
    /// <summary>
    /// Represents Splunk configuration section.
    /// </summary>
    public class SplunkLogProviderSection : ConfigurationSection
    {
        /// <summary>
        /// Gets or sets Splunk base URL value.
        /// </summary>
        [ConfigurationProperty(nameof(BaseUrl), IsRequired = false)]
        public string BaseUrl
        {
            get => (string)this[nameof(BaseUrl)];
            set => this[nameof(BaseUrl)] = value;
        }

        /// <summary>
        /// Gets or sets Splunk HTTP Event Collection Token value.
        /// </summary>
        [ConfigurationProperty(nameof(Token), IsRequired = false)]
        public string Token
        {
            get => (string)this[nameof(Token)];
            set => this[nameof(Token)] = value;
        }

        /// <summary>
        /// Gets or sets the minimal allowed logging level.
        /// </summary>
        [ConfigurationProperty(nameof(LoggingLevel), DefaultValue = LogLevel.Info)]
        public LogLevel LoggingLevel
        {
            get => (LogLevel)this[nameof(LoggingLevel)];
            set => this[nameof(LoggingLevel)] = value;
        }
    }
}