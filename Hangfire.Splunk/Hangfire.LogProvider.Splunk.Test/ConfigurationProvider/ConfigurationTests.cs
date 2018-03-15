using System.Configuration;
using Xunit;

namespace Hangfire.LogProvider.Splunk.Test.ConfigurationProvider
{
    public class ConfigurationTests
    {
        [Fact]
        public void ShouldReadSplunkConfiguration()
        {
            var section = ConfigurationManager.GetSection("SplunkLogProvider");
            Assert.NotNull(section);

            var splunkConfiguration = section as SplunkLogProviderSection;
            Assert.NotNull(splunkConfiguration);

            Assert.NotNull(splunkConfiguration.BaseUrl);
            Assert.False(string.IsNullOrWhiteSpace(splunkConfiguration.Token));
            Assert.Equal("Trace", splunkConfiguration.LoggingLevel);
            Assert.Equal("_json", splunkConfiguration.SourceType);
            Assert.Equal(3, splunkConfiguration.BucketSize);
            Assert.True(splunkConfiguration.AllowDynamicLogLevelSwitch);
        }
    }
}