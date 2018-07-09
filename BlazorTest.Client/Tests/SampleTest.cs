using NUnit.Framework;
using System.Diagnostics;

namespace BlazorTest.Client.Tests
{
    public static class SampleTests
    {
        [Test]
        public static void SampleTest()
        {            
            TestContext.WriteLine("Test output");
            EatCPU(1000);
            TestContext.WriteLine("More test output");
            EatCPU(1000);
            Assert.Pass("Pass message");
        }

        [Test]
        public static void FailingTest()
        {
            EatCPU(1000);
            Assert.Fail("Failure message");
        }

        [Test]
        public static void Warning()
        {
            EatCPU(1000);
            Assert.Warn("Warning message");
        }

        [Test]
        public static void InconclusiveTest()
        {
            EatCPU(1000);
            Assert.Inconclusive("Inconclusive message");
        }

        private static void EatCPU(int milliseconds)
        {
            for (var stopwatch = Stopwatch.StartNew(); stopwatch.ElapsedMilliseconds < milliseconds;)
            {
            }
        }
    }
}
