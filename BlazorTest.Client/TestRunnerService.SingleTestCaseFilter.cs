using NUnit.Framework.Interfaces;
using System;

namespace BlazorTest.Client
{
    public sealed partial class TestRunnerService
    {
        private sealed class SingleTestCaseFilter : ITestFilter
        {
            private readonly ITest testCaseInstance;

            public SingleTestCaseFilter(ITest testCaseInstance)
            {
                this.testCaseInstance = testCaseInstance;
            }

            public bool IsExplicitMatch(ITest test)
            {
                return test == testCaseInstance;
            }

            public bool Pass(ITest test)
            {
                return test.HasChildren || IsExplicitMatch(test);
            }

            public TNode AddToXml(TNode parentNode, bool recursive)
            {
                throw new NotImplementedException();
            }

            public TNode ToXml(bool recursive)
            {
                throw new NotImplementedException();
            }
        }
    }
}
