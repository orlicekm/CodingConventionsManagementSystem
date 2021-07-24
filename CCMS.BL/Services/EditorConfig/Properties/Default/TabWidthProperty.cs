using System;
using System.IO;
using CCMS.BL.Services.EditorConfig.Properties.Base;
using CCMS.BL.Services.EditorConfig.Properties.Helpers;

namespace CCMS.BL.Services.EditorConfig.Properties.Default
{
    /// <summary>Determines number of columns used to represent a tab character. Cannot be supported outside editor.</summary>
    public class TabWidthProperty : IProperty
    {
        public string Name => this.ToName();
        public string Description => "Determines number of columns used to represent a tab character.\nCannot be supported outside editor.";
    }
}