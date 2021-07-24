using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using CCMS.BL.Configurator;
using CCMS.BL.Services.Base;
using HtmlAgilityPack;
using Microsoft.AspNetCore.WebUtilities;

namespace CCMS.BL.Services
{
    public class HighlightService : IService, ISingleton
    {
        public string HighlightInIni(string text)
        {
            const string url = "http://hilite.me/api";
            var param = new Dictionary<string, string> {{"code", text}, {"lexer", "ini"}};
            var urlWithParams = new Uri(QueryHelpers.AddQueryString(url, param));

            var httpRequest = (HttpWebRequest) WebRequest.Create(urlWithParams);
            var httpResponse = (HttpWebResponse) httpRequest.GetResponse();
            return FormatIni(httpResponse.GetResponseStream());
        }

        private string FormatIni(Stream text)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(text);
            var result = htmlDocument.DocumentNode.SelectSingleNode("//pre")?.OuterHtml ?? string.Empty;
            result = result.Replace("<span style=\"color: #888888\">", "<span class=\"text-muted\">");
            result = result.Replace("<span style=\"color: #FF0000; background-color: #FFAAAA\">",
                "<span class=\"bg-danger\">");
            result = result.Replace("<span style=\"color: #008800; font-weight: bold\">",
                "<span name=\"section\" class=\"text-primary\" style=\"font-weight: bold\">");
            result = result.Replace("<span style=\"color: #0000CC\">", "<span>");
            result = result.Replace("<span style=\"color: #333333\">", "<span name=\"property\">");
            result = result.Replace("<span style=\"background-color: #fff0f0\">", "<span class=\"text-secondary\">");
            return result;
        }

        public string FormatPatch(string patch)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(patch);
            htmlDocument.DocumentNode.SelectNodes("//ins")?.ToList().ForEach(n =>
            {
                n.Name = "SPAN";
                n.Attributes["STYLE"].Remove();
                n.Attributes.Add("class", "bg-danger text-white");
            });
            htmlDocument.DocumentNode.SelectNodes("//del")?.ToList().ForEach(n =>
            {
                n.Name = "SPAN";
                n.Attributes["STYLE"].Remove();
                n.Attributes.Add("class", "bg-success text-white");
            });
            return htmlDocument.DocumentNode.OuterHtml;
        }
    }
}