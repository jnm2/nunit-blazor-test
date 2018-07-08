using NUnit.Common;
using System;
using System.Text;

namespace BlazorTest.Client
{
    partial class TestRunnerService
    {
        private sealed class LogAdapterWriter : ExtendedTextWriter
        {
            private static readonly Encoding DefaultEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

            private readonly AtomicLog<LogSegment> log;

            public LogAdapterWriter(AtomicLog<LogSegment> log)
            {
                this.log = log ?? throw new ArgumentNullException(nameof(log));
                NewLine = "\n";
            }

            public override Encoding Encoding => DefaultEncoding;

            public override void WriteLine()
            {
                log.Add(new LogSegment(ColorStyle.Default, null, endLine: true));
            }

            public override void Write(string value)
            {
                log.Add(new LogSegment(ColorStyle.Default, value, endLine: false));
            }

            public override void WriteLine(string value)
            {
                log.Add(new LogSegment(ColorStyle.Default, value, endLine: true));
            }

            public override void Write(ColorStyle style, string value)
            {
                log.Add(new LogSegment(style, value, endLine: false));
            }

            public override void WriteLine(ColorStyle style, string value)
            {
                log.Add(new LogSegment(style, value, endLine: true));
            }

            public override void WriteLabel(string label, object option)
            {
                WriteLabelInternal(label, option?.ToString(), endLine: false);
            }

            public override void WriteLabel(string label, object option, ColorStyle valueStyle)
            {
                WriteLabelInternal(label, option?.ToString(), endLine: false, valueStyle);
            }

            public override void WriteLabelLine(string label, object option)
            {
                WriteLabelInternal(label, option?.ToString(), endLine: true);
            }

            public override void WriteLabelLine(string label, object option, ColorStyle valueStyle)
            {
                WriteLabelInternal(label, option?.ToString(), endLine: true, valueStyle);
            }

            private void WriteLabelInternal(string label, string value, bool endLine, ColorStyle valueStyle = ColorStyle.Value)
            {
                log.AddRange(
                    new LogSegment(ColorStyle.Label, label, endLine: false),
                    new LogSegment(valueStyle, value, endLine));
            }
        }
    }
}
