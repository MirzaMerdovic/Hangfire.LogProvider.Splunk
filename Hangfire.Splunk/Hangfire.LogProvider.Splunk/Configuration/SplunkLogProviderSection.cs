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
        /// Gets or sets a Splunk default field that identifies the data structure of an event.
        /// Default value is '_json'
        /// <remarks>
        /// More info: https://docs.splunk.com/Splexicon:Sourcetype
        /// </remarks>
        /// </summary>
        [ConfigurationProperty(nameof(SourceType), DefaultValue = "_json")]
        public string SourceType
        {
            get => (string)this[nameof(SourceType)];
            set => this[nameof(SourceType)] = value;
        }

        /// <summary>
        /// Gets or sets Splunk default field that identifies the source of an event, that is, where the event originated.
        /// <remarks>
        /// More info: https://docs.splunk.com/Splexicon:Source
        /// </remarks>
        /// </summary>
        [ConfigurationProperty(nameof(Source), IsRequired = true)]
        public string Source
        {
            get => (string)this[nameof(Source)];
            set => this[nameof(Source)] = value;
        }

        /// <summary>
        /// Gets or sets the name of Splunk index. 
        /// Default value is 'main'
        /// <remarks>
        /// More info: https://docs.splunk.com/Splexicon:Index
        /// </remarks>
        /// </summary>
        [ConfigurationProperty(nameof(Index), DefaultValue = "main")]
        public string Index
        {
            get => (string)this[nameof(Index)];
            set => this[nameof(Index)] = value;
        }

        /// <summary>
        /// Gets or sets the minimal allowed logging level.
        /// Default value is 'Warning'
        /// </summary>
        [ConfigurationProperty(nameof(LoggingLevel), DefaultValue = LogLevel.Warn)]
        public LogLevel LoggingLevel
        {
            get => (LogLevel)this[nameof(LoggingLevel)];
            set => this[nameof(LoggingLevel)] = value;
        }

        /// <summary>
        /// Gets or sets the maximum number of messages that will be stored before logger pushes them to Splunk.
        /// Default value is '10'
        /// </summary>
        [ConfigurationProperty(nameof(BucketSize), DefaultValue = 10)]
        public int BucketSize
        {
            get => (int)this[nameof(BucketSize)];
            set => this[nameof(BucketSize)] = value;
        }
    }
}