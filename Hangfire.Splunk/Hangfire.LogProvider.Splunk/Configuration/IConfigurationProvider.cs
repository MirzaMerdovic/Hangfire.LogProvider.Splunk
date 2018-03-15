using System;

namespace Hangfire.LogProvider.Splunk.Configuration
{
    /// <summary>
    /// Represent configuration contract that needs to be implemented.
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Gets Splunk's HEC base URL.
        /// </summary>
        Uri BaseUrl { get; }

        /// <summary>
        /// Gets Splunk's HTTP HEC Token value.
        /// </summary>
        string Token { get; }

        /// <summary>
        /// Gets Splunk's default field that identifies the data structure of an event.
        /// Default value is '_json'
        /// <remarks>
        /// More info: https://docs.splunk.com/Splexicon:Sourcetype
        /// </remarks>
        /// </summary>
        string SourceType { get; }

        /// <summary>
        /// Gets the name of Splunk index. 
        /// Default value is 'main'
        /// <remarks>
        /// More info: https://docs.splunk.com/Splexicon:Index
        /// </remarks>
        /// </summary>
        string Index { get; }

        /// <summary>
        /// Gets the minimal allowed logging level.
        /// Default value is 'Warning'
        /// </summary>
        string MinimumLogLevel { get; }

        /// <summary>
        /// Gets the maximum number of messages that will be stored before logger pushes them to Splunk.
        /// Default value is '10'
        /// </summary>
        int BucketSize { get; }

        /// <summary>
        /// Gets the flag that determine whether the dynamic log level switch functionality should be allowed or not.
        /// </summary>
        bool AllowDynamicLogLevelSwitch { get; }

        /// <summary>
        /// Tries to retrieve minimum log level value.
        /// <remarks>
        /// This method allows dynamic log level switching.
        /// </remarks>
        /// </summary>
        /// <returns></returns>
        string CheckMinimumLogLevel();
    }
}
