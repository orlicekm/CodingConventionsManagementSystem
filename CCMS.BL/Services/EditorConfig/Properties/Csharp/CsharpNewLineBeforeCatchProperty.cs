using System.Collections.Generic;
using System.IO;
using System.Linq;
using CCMS.BL.Services.EditorConfig.Check;
using CCMS.BL.Services.EditorConfig.Enums;
using CCMS.BL.Services.EditorConfig.Import;
using CCMS.BL.Services.EditorConfig.Properties.Base;
using CCMS.BL.Services.EditorConfig.Properties.Default.Enums;
using CCMS.BL.Services.EditorConfig.Properties.Exceptions;
using CCMS.BL.Services.EditorConfig.Properties.Helpers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CCMS.BL.Services.EditorConfig.Properties.Csharp
{
    /// <summary>Determines if "catch" statement is on newline.</summary>
    public class CsharpNewLineBeforeCatchProperty : IProperty, ICheckable, IImportable
    {
        public string Name => this.ToName();
        public string Description => "Determines if \"catch\" statement is on newline.";
        public string AllowedValues => typeof(EBool).ToAllowedValue();
        public string AllowedFileTypes => "All (*), but can parse only C# code";
        public string DefaultSection => "*.cs";

        public CheckResult Check(string value, FileInfo file)
        {
            if (!typeof(EBool).ToStringArray().Contains(value))
                throw new UnsupportedPropertyValueException();
            var text = File.ReadAllText(file.FullName);
            var root = CSharpSyntaxTree.ParseText(text).GetCompilationUnitRoot();
            var clausules = root.DescendantNodes().OfType<CatchClauseSyntax>();
            var failedLines = new List<int>();
            var result = new CheckResult
            {
                State = ECheckState.Success,
                Message = string.Empty
            };
            foreach (var clausule in clausules)
            {
                if (value == "true" && !IsEndOfLineBefore(clausule))
                    failedLines.Add(clausule.CatchKeyword.GetLocation().GetLineSpan().StartLinePosition.Line);

                if (value == "false" && IsEndOfLineBefore(clausule))
                    failedLines.Add(clausule.CatchKeyword.GetLocation().GetLineSpan().StartLinePosition.Line);
            }

            failedLines = failedLines.Distinct().ToList();
            if (failedLines.Any())
            {
                result.State = ECheckState.Fail;
                if (value == "true") result.Message += $"No newline found on the line(s) {string.Join(", ", failedLines.Select(n => n + 1))}.";
                if (value == "false") result.Message += $"Newline found on the line(s) {string.Join(", ", failedLines)}.";
            }
            return result;
        }

        public ImportResult Import(FileInfo file)
        {
            var text = File.ReadAllText(file.FullName);
            var result = new ImportResult() { Result = new Dictionary<string, double>() };
            var root = CSharpSyntaxTree.ParseText(text).GetCompilationUnitRoot();
            var clausules = root.DescendantNodes().OfType<CatchClauseSyntax>();
            foreach (var clausule in clausules)
            {
                if (IsEndOfLineBefore(clausule))
                {
                    if (result.Result.ContainsKey("true"))
                        result.Result["true"] += 1;
                    else result.Result["true"] = 1;
                }
                else
                {
                    if (result.Result.ContainsKey("false"))
                        result.Result["false"] += 1;
                    else result.Result["false"] = 1;
                }
            }
            return result;
        }

        private bool IsEndOfLineBefore(CatchClauseSyntax clause)
        {
            return clause.CatchKeyword.LeadingTrivia.Any(t => t.Kind() is SyntaxKind.EndOfLineTrivia) ||
                   clause.CatchKeyword.GetPreviousToken().TrailingTrivia
                       .Any(t => t.Kind() is SyntaxKind.EndOfLineTrivia);
        }
    }
}