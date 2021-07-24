using System.Reflection;
using CCMS.BL.Configurator;
using CCMS.BL.Services.Base;
// ReSharper disable once RedundantUsingDirective
using Microsoft.CodeAnalysis;

namespace CCMS.BL.Services.EditorConfig
{
    public class RoslynInteropService : IService, ISingleton
    {
        public bool IsSectionMatchingFile(string sectionName, string fileRelativePath)
        {
            var assembly =
                Assembly.Load(
                    "Microsoft.CodeAnalysis, Version=3.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            var type = assembly.GetType("Microsoft.CodeAnalysis.AnalyzerConfig");
            var method = type.GetMethod("TryCreateSectionNameMatcher", BindingFlags.NonPublic | BindingFlags.Static);
            var sectionNameMatcher = method.Invoke(null, new[] {sectionName});
            var isMatchMethod = sectionNameMatcher.GetType().GetMethod("IsMatch");
            return (bool) isMatchMethod.Invoke(sectionNameMatcher, new[] {fileRelativePath});
        }
    }
}