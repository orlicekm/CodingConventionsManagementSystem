using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace CCMS.BL.Services.Helpers
{
    //inspired by https://stackoverflow.com/questions/29995333/convert-render-html-to-text-with-correct-line-breaks/30086182
    public static class HtmlPlainTextExtension
    {
        public enum ToPlainTextState
        {
            StartLine = 0,
            NotWhiteSpace,
            WhiteSpace
        }

        private static readonly HashSet<string> InlineTags = new()
        {
            "b", "big", "i", "small", "tt", "abbr", "acronym",
            "cite", "code", "dfn", "em", "kbd", "strong", "samp",
            "var", "a", "bdo", "br", "img", "map", "object", "q",
            "script", "span", "sub", "sup", "button", "input", "label",
            "select", "textarea"
        };

        private static readonly HashSet<string> NonVisibleTags = new()
        {
            "script", "style"
        };

        public static string ToPlainText(this string text)
        {
            if (text == null) return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            return doc.ToPlainText();
        }

        public static string ToPlainText(this HtmlDocument doc)
        {
            var builder = new StringBuilder();
            var state = ToPlainTextState.StartLine;

            Plain(builder, ref state, new[] {doc.DocumentNode});
            return builder.ToString();
        }

        private static void Plain(StringBuilder builder, ref ToPlainTextState state,
            IEnumerable<HtmlNode> nodes)
        {
            foreach (var node in nodes)
                if (node is HtmlTextNode text)
                {
                    Process(builder, ref state, HtmlEntity.DeEntitize(text.Text).ToCharArray());
                }
                else
                {
                    var tag = node.Name.ToLower();

                    if (tag == "br")
                    {
                        builder.AppendLine();
                        state = ToPlainTextState.StartLine;
                    }
                    else if (NonVisibleTags.Contains(tag))
                    {
                    }
                    else if (InlineTags.Contains(tag))
                    {
                        Plain(builder, ref state, node.ChildNodes);
                    }
                    else
                    {
                        if (state != ToPlainTextState.StartLine)
                        {
                            builder.AppendLine();
                            state = ToPlainTextState.StartLine;
                        }

                        Plain(builder, ref state, node.ChildNodes);
                        if (state != ToPlainTextState.StartLine)
                        {
                            builder.AppendLine();
                            state = ToPlainTextState.StartLine;
                        }
                    }
                }
        }

        private static void Process(StringBuilder builder, ref ToPlainTextState state, params char[] chars)
        {
            foreach (var ch in chars)
                if (char.IsWhiteSpace(ch))
                {
                    if (IsHardSpace(ch))
                    {
                        if (state == ToPlainTextState.WhiteSpace)
                            builder.Append(' ');
                        builder.Append(' ');
                        state = ToPlainTextState.NotWhiteSpace;
                    }
                    else
                    {
                        if (state == ToPlainTextState.NotWhiteSpace)
                            state = ToPlainTextState.WhiteSpace;
                    }
                }
                else
                {
                    if (state == ToPlainTextState.WhiteSpace)
                        builder.Append(' ');
                    builder.Append(ch);
                    state = ToPlainTextState.NotWhiteSpace;
                }
        }

        private static bool IsHardSpace(char ch)
        {
            return ch == 0xA0 || ch == 0x2007 || ch == 0x202F;
        }
    }
}