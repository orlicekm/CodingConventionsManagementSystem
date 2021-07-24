using GalaSoft.MvvmLight;

namespace CCMS.BL.ViewModels.Observables
{
    public class ExecuteMessageObservable : ObservableObject
    {
        public enum EState
        {
            Running,
            Success,
            Fail
        }

        private EState state;
        private string text;

        public ExecuteMessageObservable(string text)
        {
            state = EState.Running;
            this.text = text;
        }

        public EState State
        {
            get => state;
            set => Set(() => State, ref state, value);
        }

        public string Text
        {
            get => text;
            set => Set(() => Text, ref text, value);
        }
    }
}