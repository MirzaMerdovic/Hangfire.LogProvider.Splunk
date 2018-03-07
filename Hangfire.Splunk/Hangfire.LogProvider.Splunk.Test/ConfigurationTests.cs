using System.Configuration;
using Hangfire.Common;
using Hangfire.Logging;
using Hangfire.LogProvider.Splunk.Configuration;
using Hangfire.SqlServer;
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
        }

        [Fact]
        public void ShouldRun()
        {
            TestCode();
        }

        private static void TestCode()
        {
            var storage = new SqlServerStorage("Server=localhost;Initial Catalog=NetEntCasino.Job.Database;User ID=sa;Password=Password1!", new SqlServerStorageOptions {PrepareSchemaIfNecessary = false});
            GlobalConfiguration.Configuration
                .UseSplunkLogProvider()
                .UseStorage(storage);

            var components = storage.GetComponents();
            var connection = storage.GetConnection();
            var api = storage.GetMonitoringApi();

            using (new BackgroundJobServer())
            {
                var manager = new RecurringJobManager(storage);

                manager.AddOrUpdate("42", new Job(typeof(TestJob).GetMethod("Run")), Cron.Minutely());
                manager.Trigger("TestJob");

                var j = api.JobDetails("42");
                var job = connection.GetStateData("42");
                Assert.NotNull(job);
            }
        }

        public class TestJob
        {
            public void Run()
            {
            }
        }
    }
}
