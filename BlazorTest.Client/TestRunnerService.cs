using NUnit;
using NUnit.Common;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnitLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorTest.Client
{
    public sealed partial class TestRunnerService : IDisposable
    {
        private readonly NUnitTestAssemblyRunner runner;
        private readonly LogAdapterWriter writer;
        private readonly Assembly assembly;
        private readonly AtomicLog<LogSegment> log = new AtomicLog<LogSegment>();

        public TestRunnerService(Assembly assembly)
        {
            runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            writer = new LogAdapterWriter(log);
            this.assembly = assembly;
        }

        public void Dispose()
        {
            runner.StopRun(force: true);
        }

        public void Start()
        {
            if (runner.IsTestRunning) return;
            Task.Run(RunTests);
        }

        private async Task RunTests()
        {
            try
            {
                var assemblySuite = runner.Load(assembly, new Dictionary<string, object>
                {
                    [FrameworkPackageSettings.NumberOfTestWorkers] = 0,
                    [FrameworkPackageSettings.SynchronousEvents] = true,
                    [FrameworkPackageSettings.RunOnMainThread] = true
                });
                var textUI = new TextUI(writer, reader: null, options: new NUnitLiteOptions("--workers=0"));
                textUI.DisplayHeader();
                textUI.DisplayTestFiles(new string[] { assembly.FullName });

                var results = new List<ITestResult>();

                foreach (var test in assemblySuite.Tests.SelectManyRecursive(suite => suite.Tests))
                {
                    if (test.HasChildren) continue;

                    await Task.Delay(1); // Allow UI to respond

                    results.Add(runner.Run(new TextUIAdapterListener(textUI), new SingleTestCaseFilter(test)));
                }

                var result = MergeTestResults(results);
                var summary = new ResultSummary(result);

                if (summary.ExplicitCount + summary.SkipCount + summary.IgnoreCount > 0)
                    textUI.DisplayNotRunReport(result);

                textUI.DisplayErrorsFailuresAndWarningsReport(result);
                textUI.DisplayRunSettings();
                textUI.DisplaySummaryReport(summary);

                OnTestRunEnded();
            }
            catch (Exception ex)
            {
                writer.WriteLine(ColorStyle.Error, "Unhandled runner exception: ");
                writer.WriteLine(ColorStyle.Error, ex.ToString());
            }
        }

        private void OnTestRunEnded()
        {
        }

        private static ITestResult MergeTestResults(IEnumerable<ITestResult> partialResults)
        {
            if (partialResults == null) throw new ArgumentNullException(nameof(partialResults));

            var nodesToMerge = new List<ITestResult>();

            using (var en = partialResults.GetEnumerator())
            {
                if (!en.MoveNext()) throw new ArgumentException("There must be at least one result.", nameof(partialResults));

                var first = en.Current;
                if (!en.MoveNext()) return first;

                nodesToMerge.Add(first);
                do
                {
                    var next = en.Current;
                    if (first.Test != next.Test)
                        throw new InvalidOperationException("Result nodes for different tests cannot be merged.");

                    nodesToMerge.Add(next);
                } while (en.MoveNext());
            }

            var merged = new TestSuiteResult((TestSuite)nodesToMerge[0].Test);

            foreach (var childNodesToMerge in nodesToMerge.SelectMany(n => n.Children).GroupBy(c => c.Test))
            {
                merged.AddResult(MergeTestResults(childNodesToMerge));
            }

            return merged;
        }

        public IDisposable SubscribeLog(Action<IReadOnlyList<LogSegment>> onSegmentsAdded)
        {
            if (onSegmentsAdded == null) throw new ArgumentNullException(nameof(onSegmentsAdded));

            return log.Subscribe(onSegmentsAdded);
        }
    }
}
