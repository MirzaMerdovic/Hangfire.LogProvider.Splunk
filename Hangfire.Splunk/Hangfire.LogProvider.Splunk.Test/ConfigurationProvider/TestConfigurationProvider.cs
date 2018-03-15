using Hangfire.LogProvider.Splunk.Configuration;
using System;
using System.Configuration;

namespace Hangfire.LogProvider.Splunk.Test.ConfigurationProvider
{
    internal class TestConfigurationProvider : IConfigurationProvider
    {
        private readonly SplunkLogProviderSection _section;

        public TestConfigurationProvider()
        {
            _section = (SplunkLogProviderSection)ConfigurationManager.GetSection("SplunkLogProvider");
        }

        public Uri BaseUrl => _section.BaseUrl;

        public string Token => _section.Token;

        public string SourceType => _section.SourceType;

        public string Index => _section.Index;

        public string MinimumLogLevel => _section.LoggingLevel;

        public int BucketSize => _section.BucketSize;

        public bool AllowDynamicLogLevelSwitch => _section.AllowDynamicLogLevelSwitch;

        public string CheckMinimumLogLevel()
        {
            return ((SplunkLogProviderSection)ConfigurationManager.GetSection("SplunkLogProvider")).LoggingLevel;
        }
    }
}
