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

            Assert.IsTrue(content.EndsWith(expectedContent));
        }

        [Test]
        public void FieLogger_FlushShutdown()
        {
            Assert.Pass();
        }

        [Test]
        public void FileLogger_ForcedShutdown()
        {
            Assert.Pass();
        }

        [Test]
        public void FileLogger_CreateNewFileAtMidnight()
        {

        }
    }
}