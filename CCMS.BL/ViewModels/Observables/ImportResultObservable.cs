using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace CCMS.BL.ViewModels.Observables
{
    public class ImportResultObservable : ObservableObject
    {
        private ValuablePropertyObservable property;
        private string selectedValue;
        private IList<ImportValueObservable> importValues;

        public class ImportValueObservable: ObservableObject
        {
            private string section;
            private double ratio;

            public string Section
            {
                get => section;
                set => Set(() => Section, ref section, value);
            }

            public double Ratio
            {
                get => ratio;
                set => Set(() => Ratio, ref ratio, value);
            }

            public string Text => ratio == 0 ? Section : $"{Section} ({Ratio:0.##}%)";
        }

        public ValuablePropertyObservable Property
        {
            get => property;
            set => Set(() => Property, ref property, value);
        }

        public string SelectedValue
        {
            get => selectedValue;
            set => Set(() => SelectedValue, ref selectedValue, value);
        }

        public IList<ImportValueObservable> ImportValues
        {
            get => importValues;
            set => Set(() => ImportValues, ref importValues, value);
        }
    }
}