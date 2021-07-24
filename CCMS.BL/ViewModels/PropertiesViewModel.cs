using System;
using System.Collections.Generic;
using System.Linq;
using CCMS.BL.Configurator;
using CCMS.BL.Services.EditorConfig.Properties.Base;
using CCMS.BL.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;

namespace CCMS.BL.ViewModels
{
    public class PropertiesViewModel : BaseViewModel, IScoped
    {
        private readonly ICollection<IProperty> properties;

        public PropertiesViewModel(IServiceProvider serviceProvider)
        {
            Properties = serviceProvider.GetServices<IProperty>().ToList();
            ImportableProperties = Properties.Where(p => p is IImportable).ToList();
        }

        public ICollection<IProperty> Properties
        {
            get => properties;
            init => SetPropertyValue(ref properties, value);
        }

        public ICollection<IProperty> ImportableProperties { get; private set; }
    }
}