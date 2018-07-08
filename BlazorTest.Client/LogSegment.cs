using NUnit.Common;

namespace BlazorTest.Client
{
    public readonly struct LogSegment
    {
        public ColorStyle ColorStyle { get; }
        public string Text { get; }
        public bool EndLine { get; }

        public LogSegment(ColorStyle colorStyle, string text, bool endLine)
        {
            ColorStyle = colorStyle;
            Text = text;
            EndLine = endLine;
        }
    }
}
