using System.ComponentModel;

namespace BuildIt.States.Interfaces
{
    public interface IHasStateData
    {
        INotifyPropertyChanged CurrentStateData { get; }
    }
}