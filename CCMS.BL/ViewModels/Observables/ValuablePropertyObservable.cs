using System.Collections.Generic;
using System.Linq;
using CCMS.BL.Services.EditorConfig.Properties.Base;
using GalaSoft.MvvmLight;

namespace CCMS.BL.ViewModels.Observables
{
    public class ValuablePropertyObservable : ObservableObject
    {
        private string name;
        private readonly ICollection<IProperty> allProperties;
        private string value;

        public ValuablePropertyObservable(ICollection<IProperty> allProperties, IProperty property, string value)
        {
            name = property.Name;
            this.allProperties = allProperties;
            this.value = value;
        }

        public string Name
        {
            get => name;
            set => Set(() => Name, ref name, value);
        }

        public string Value
        {
            get => value;
            set => Set(() => Value, ref this.value, value);
        }

        public IProperty Property => allProperties.FirstOrDefault(p => p.Name == Name);
    }
}