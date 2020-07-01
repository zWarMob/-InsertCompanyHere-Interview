using LogComponent;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace LogComponentTests
{
    public class Tests
    {
        FileLogger logger;

        [SetUp]
        public void Setup()
        {
            logger = new FileLogger();
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
        public void FieLogger_Log()
        {
            string testContent = "Let's see if this really works";

            LogLine expectedLine = new LogLine(testContent);

            string expectedContent = expectedLine.ToString().Substring(LogLine.timestampFormat.Length);

            logger.WriteLog(testContent);

            logger.Stop_With_Flush();

            Thread.Sleep(1500);

            var file = (File.OpenText(Path.Combine(logger.LogPath, logger.OpenLogFileName)));

            var content = file.ReadToEnd();

            file.Close();

            Assert.IsTrue(content.EndsWith(expectedContent));
        }

        [Test]
        public void FieLogger_FlushShutdown()
        {
            string testContent = "Number with Flush: 1";

            LogLine expectedLine = new LogLine(testContent);

            string expectedContent = expectedLine.ToString().Substring(LogLine.timestampFormat.Length);

            for (int i = 10000; i > 0; i--)
            {
                logger.WriteLog("Number with Flush: " + i.ToString());
            }

            logger.Stop_With_Flush();

            Thread.Sleep(1500);

            var file = (File.OpenText(Path.Combine(logger.LogPath, logger.OpenLogFileName)));

            var content = file.ReadToEnd();
            
            file.Close();

            Assert.IsTrue(content.EndsWith(expectedContent));
        }

        [Test]
        public void FileLogger_ForcedShutdown()
        {
            string testContent = "Number with Flush: 1";

            LogLine expectedLine = new LogLine(testContent);

            string expectedContent = expectedLine.ToString().Substring(LogLine.timestampFormat.Length);

            for (int i = 10000; i > 0; i--)
            {
                logger.WriteLog("Number with Flush: " + i.ToString());
            }

            logger.Stop_Without_Flush();

            Thread.Sleep(1500);

            var file = (File.OpenText(Path.Combine(logger.LogPath, logger.OpenLogFileName)));

            var content = file.ReadToEnd();
            
            file.Close();

            Assert.IsFalse(content.EndsWith(expectedContent));
        }
    }
}