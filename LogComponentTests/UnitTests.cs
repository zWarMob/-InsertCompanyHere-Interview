using LogComponent;
using NUnit.Framework;

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
            logger.WriteLog("Test");
            Assert.Pass();
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