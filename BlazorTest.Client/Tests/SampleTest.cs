using NUnit.Framework;

namespace BlazorTest.Client.Tests
{
    public static class SampleTests
    {
        [Test]
        public static void SampleTest()
        {
            TestContext.WriteLine("Test output");
            Assert.Pass("Pass message");
        }

        [Test]
        public static void FailingTest()
        {
            Assert.Fail("Failure message");
        }

        [Test]
        public static void Warning()
        {
            Assert.Warn("Warning message");
        }

        [Test]
        public static void InconclusiveTest()
        {
            Assert.Inconclusive("Inconclusive message");
        }
    }
}
