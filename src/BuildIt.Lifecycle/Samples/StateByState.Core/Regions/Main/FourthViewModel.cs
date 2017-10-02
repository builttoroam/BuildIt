using BuildIt.Lifecycle.States.ViewModel;

namespace StateByState.Regions.Main
{
    public class FourthViewModel : BaseStateManagerViewModel
    {
        private string initValue;

        public string InitValue
        {
            get { return initValue; }
            set
            {
                initValue = value;
                OnPropertyChanged();
            }
        }
    }
}