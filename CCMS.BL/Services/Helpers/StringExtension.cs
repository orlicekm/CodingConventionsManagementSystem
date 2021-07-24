using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CCMS.BL.Services.Helpers
{
    public static class StringExtension
    {
        public static string ToUnscriptedHtml(this string str)
        {
            return Regex.Replace(str, @"<script[^>]*>[\s\S]*?</script>", "");
        }

        public static string Shrink(this string str, int count)
        {
            if (str == null) return null;
            var subStr = new string(str.Take(count).ToArray());
            return subStr.Length == count ? subStr + "..." : subStr;
        }

        public static string Shrink(this string str)
        {
            return Shrink(str, 200);
        }

        public static int CountProperties(this string str)
        {
            return Regex.Matches(new HighlightService().HighlightInIni(str), "<span name=\"property\"").Count;
        }
        public static int CountSections(this string str)
        {
            return Regex.Matches(new HighlightService().HighlightInIni(str), "<span name=\"section\"").Count;
        }

        public static IEnumerable<string> ToLines(this string str)
        {
            return Regex.Split(str, "\r\n|\r|\n");
        }
    }
}