using System;
using System.Configuration;
using System.Diagnostics;
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
        }

        [Fact]
        public void ShouldDisposeSplunkLogProvider()
        {
            GlobalConfiguration.Configuration
                .UseSplunkLogProvider(out var disposable);

            disposable.Dispose();

            Assert.IsType<SplunkLogProvider>(disposable);
            var isDisposed = (bool) disposable.GetType().GetProperty("Disposed").GetValue(disposable);
            Assert.True(isDisposed);
        }

        [Fact(Skip = "ShouldDisposeSplunkProvider and this one cannot run in parallel.")]
        public void ShouldThrowObjectDisposedException()
        {
            Assert.Throws<AggregateException>(() => TestCode());
        }

        private static void TestCode()
        {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage("Server=localhost;Initial Catalog=Jobs;User ID=sa;Password=Password1!")
                .UseSplunkLogProvider(out var disposable);

            using (new BackgroundJobServer())
            {
                RecurringJob.AddOrUpdate("TestJob", () => Debug.WriteLine("Testing testint"), Cron.Yearly);
                RecurringJob.Trigger("TestJob");

                disposable.Dispose();
            }
        }
    }
}
