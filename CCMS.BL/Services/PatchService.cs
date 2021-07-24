using System;
using System.Collections.Generic;
using System.Linq;
using CCMS.BL.Configurator;
using CCMS.BL.Models.List;
using CCMS.BL.Services.Base;
using CCMS.BL.Services.Helpers;
using DiffMatchPatch;

namespace CCMS.BL.Services
{
    public class PatchService : IService, ISingleton
    {
        public string GeneratePatch(string oldText, string newText, bool isHtml = false)
        {
            var dmp = DiffMatchPatchModule.Default;

            var diffs = isHtml
                ? dmp.DiffMain(newText.ToPlainText(), oldText.ToPlainText())
                : dmp.DiffMain(newText, oldText);
            return dmp.DiffToDelta(diffs);
        }

        public IEnumerable<(string, PatchListModel)> GetTexts(ICollection<PatchListModel> patches, string text,
            bool isHtml = false)
        {
            var dmp = DiffMatchPatchModule.Default;
            patches = patches.OrderByDescending(p => p.CreatedAt).ToList();
            var lastPatch = isHtml ? text.ToPlainText() : text;
            foreach (var patch in patches)
            {
                var resultDiff = dmp.DiffFromDelta(lastPatch, patch.Patch);
                lastPatch = dmp.DiffText2(resultDiff);
                yield return (dmp.DiffPrettyHtml(resultDiff), patch);
            }
        }
    }
}