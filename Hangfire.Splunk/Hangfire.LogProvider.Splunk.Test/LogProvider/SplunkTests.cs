using System.Threading;
using System.Threading.Tasks;
using Hangfire.Logging;
using Hangfire.LogProvider.Splunk.Test.ConfigurationProvider;
using Xunit;

namespace Hangfire.LogProvider.Splunk.Test.LogProvider
{
    public class SplunkTests
    {
        [Fact]
        public void ShouldRun()
        {
            ILogProvider logProvider = new SplunkLogProvider(new TestConfigurationProvider());
            ILog logger = logProvider.GetLogger(null);
            logger.Warn("See you in Splunk search 1-:");
            logger.Warn("See you in Splunk search 2-:");
            logger.Warn("See you in Splunk search 3-:");
        }
    }

    public class SplunkTest2
    {
        [Fact]
        public void ShouldRun2()
        {
            ILogProvider logProvider = new SplunkLogProvider(new TestConfigurationProvider());
            ILog logger = logProvider.GetLogger(null);
            logger.Warn("See you in Splunk search 1-:");
            Thread.Sleep(50);
            logger.Warn("See you in Splunk search 2-:");
            Thread.Sleep(130);
            logger.Warn("See you in Splunk search 3-:");
        }
    }

    public class SplunkTest3
    {
        [Fact]
        public void ShouldRun3()
        {
            ILogProvider logProvider = new SplunkLogProvider(new TestConfigurationProvider());
            ILog logger = logProvider.GetLogger(null);
            logger.Warn("See you in Splunk search 1-:");
            Thread.Sleep(145);
            logger.Warn("See you in Splunk search 2-:");
            Thread.Sleep(145);
            logger.Warn("See you in Splunk search 3-:");
        }
    }

    public class SplunkTest4
    {
        [Fact]
        public void ShouldRun4()
        {
            ILogProvider logProvider = new SplunkLogProvider(new TestConfigurationProvider());
            ILog logger = logProvider.GetLogger(null);
            logger.Warn("See you in Splunk search 1-:");
            logger.Warn("See you in Splunk search 2-:");
            logger.Warn("See you in Splunk search 3-:");
            logger.Warn("See you in Splunk search 4-:");
            logger.Warn("See you in Splunk search 5-:");
            Thread.Sleep(200);
            logger.Warn("See you in Splunk search 6-:");
        }
    }

    public class SplunkTest5
    {
        [Fact]
        public async Task ShouldFlush()
        {
            ILogProvider logProvider = new SplunkLogProvider(new TestConfigurationProvider());
            ILog logger = logProvider.GetLogger("Test");
            logger.Warn("Flushing to Splunk");

            await SplunkLogProvider.Flush();

            Thread.Sleep(5000);

            await SplunkLogProvider.Flush();
        }
    }
}
