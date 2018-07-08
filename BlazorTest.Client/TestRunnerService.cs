using NUnit;
using NUnit.Common;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnitLite;
using System;
using System.Collections.Generic;
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

            Task.Run(() =>
            {
                try
                {
                    runner.Load(assembly, new Dictionary<string, object>
                    {
                        [FrameworkPackageSettings.NumberOfTestWorkers] = 0,
                        [FrameworkPackageSettings.SynchronousEvents] = true,
                        [FrameworkPackageSettings.RunOnMainThread] = true
                    });

                    var textUI = new TextUI(writer, reader: null, options: new NUnitLiteOptions("--workers=0"));
                    textUI.DisplayHeader();
                    textUI.DisplayTestFiles(new string[] { assembly.FullName });

                    var result = runner.Run(listener: new TextUIAdapterListener(textUI), TestFilter.Empty);
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
            });
        }

        private void OnTestRunEnded()
        {
        }

        public IDisposable SubscribeLog(Action<IReadOnlyList<LogSegment>> onSegmentsAdded)
        {
            if (onSegmentsAdded == null) throw new ArgumentNullException(nameof(onSegmentsAdded));

            return log.Subscribe(onSegmentsAdded);
        }
    }
}
