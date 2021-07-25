using CCMS.BL.Configurator;

namespace CCMS.BL.Services.EditorConfig.Properties.Base
{
    /// <summary>Base interface for property.</summary>
    public interface IProperty : ISingleton<IProperty>
    {
        /// <summary>Unique property name.</summary>
        public string Name { get; }

        /// <summary>User-friendly description of property that will be shown in properties page.</summary>
        public string Description { get; }
    }
}