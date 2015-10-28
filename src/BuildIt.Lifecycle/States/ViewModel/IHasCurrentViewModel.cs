using System.ComponentModel;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public interface IHasCurrentViewModel
    {
        INotifyPropertyChanged CurrentViewModel { get; }
    }
}