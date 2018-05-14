using System.ComponentModel;

namespace BuildIt
{
    /// <inheritdoc />
    /// <summary>
    /// Extends INotifyPropertyChanged to include public method that can
    /// be used to raise the PropertyChanged event
    /// </summary>
    public interface IRaisePropertyChanged : INotifyPropertyChanged
    {
        /// <summary>
        /// Method to invoke PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        void RaisePropertyChanged(string propertyName);
    }
}