using NUnit.Framework.Interfaces;
using NUnitLite;
using System;

namespace BlazorTest.Client
{
    partial class TestRunnerService
    {
        private sealed class TextUIAdapterListener : ITestListener
        {
            private readonly TextUI textUI;

            public TextUIAdapterListener(TextUI textUI)
            {
                this.textUI = textUI ?? throw new ArgumentNullException(nameof(textUI));
            }

            public void TestFinished(ITestResult result) => textUI.TestFinished(result);

            public void TestOutput(TestOutput output) => textUI.TestOutput(output);

            public void TestStarted(ITest test) => textUI.TestStarted(test);
        }
    }
}
