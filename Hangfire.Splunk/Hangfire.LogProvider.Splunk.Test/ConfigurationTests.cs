using System.Configuration;
using System.Threading.Tasks;
using Hangfire.Logging;
using Hangfire.LogProvider.Splunk.Configuration;
using Xunit;

namespace Hangfire.LogProvider.Splunk.Test
{
    public class UnitTest1
    {
        [Fact]
        public void ShouldReadSplunkConfiguration()
        {
            var section = ConfigurationManager.GetSection("SplunkLogProvider");
            Assert.NotNull(section);

            var splunkConfiguration = section as SplunkLogProviderSection;
            Assert.NotNull(splunkConfiguration);

            Assert.False(string.IsNullOrWhiteSpace(splunkConfiguration.BaseUrl));
            Assert.False(string.IsNullOrWhiteSpace(splunkConfiguration.Token));
            Assert.Equal(LogLevel.Trace, splunkConfiguration.LoggingLevel);
            Assert.Equal("Hangfire.LogProvider.Splunk.Test", splunkConfiguration.Source);
            Assert.Equal("_json", splunkConfiguration.SourceType);
            Assert.Equal(3, splunkConfiguration.BucketSize);
        }

        [Fact]
        public void ShouldRun()
        {
            ILogProvider logProvider = new SplunkLogProvider((SplunkLogProviderSection)ConfigurationManager.GetSection("SplunkLogProvider"));
            ILog logger = logProvider.GetLogger(null);
            logger.Warn("See you in Splunk search 1-:");
            logger.Warn("See you in Splunk search 2-:");
            logger.Warn("See you in Splunk search 3-:");
        }

        [Fact]
        public async Task ShouldFlush()
        {
            ILogProvider logProvider = new SplunkLogProvider((SplunkLogProviderSection)ConfigurationManager.GetSection("SplunkLogProvider"));
            ILog logger = logProvider.GetLogger(null);
            logger.Warn("Flushing to Splunk");

            await SplunkLogProvider.Flush();
        }
    }
}
