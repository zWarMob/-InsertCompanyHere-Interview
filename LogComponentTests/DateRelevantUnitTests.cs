using LogComponent;
using LogComponentCore.DateTimeProvider;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace LogComponentTests
{
    class DateRelevantUnitTests
    {
        FileLogger logger;
        DateTimeProvider timeProvider;

        [SetUp]
        public void Setup()
        {
            timeProvider = new DateTimeProvider();
            logger = new FileLogger(timeProvider);
        }

        [TearDown]
        public void Cleanup()
        {
            DirectoryInfo di = new DirectoryInfo(logger.LogPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        [Test]
        public void FileLogger_CreateNewFileAtMidnight()
        {
            timeProvider.SetDateTime( DateTime.Today.AddHours(23 - DateTime.Today.Hour)
                                                    .AddMinutes(59 - DateTime.Today.Minute)
                                                    .AddSeconds(58 - DateTime.Today.Second));

            for(var i = 0; i < 5; i++)
            {
                logger.WriteLog("Has the world ended yet?");
                Thread.Sleep(1000);
            }

            logger.Stop_Without_Flush();

            Thread.Sleep(1000);

            DirectoryInfo loggerDirectory = new DirectoryInfo(logger.LogPath);

            if (loggerDirectory.GetFiles().Length > 1)
            {
                Assert.Pass("We live for another day!");
            }
            else
            {
                Assert.Fail("Badly written graffiti: 'Michael Bay has been here'");
            }
        }
    }
}
