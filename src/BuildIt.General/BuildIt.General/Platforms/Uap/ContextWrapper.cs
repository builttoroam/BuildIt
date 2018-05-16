using System;
using Windows.UI.Xaml;

namespace BuildIt.UI
{
    /// <summary>
    /// Wraps data context for an element so that change events can be propagated when it changes
    /// </summary>
    /// <typeparam name="T">The type of the view model data</typeparam>
    public class ContextWrapper<T> : NotifyBase
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextWrapper{T}"/> class.
        /// </summary>
        /// <param name="element">The on screen element to wrap</param>
        public ContextWrapper(FrameworkElement element)
        {
            Element = element;
            element.DataContextChanged += Element_DataContextChanged;
        }

        /// <summary>
        /// Gets or sets the on screen element to wrap and monitor
        /// </summary>
        public FrameworkElement Element { get; set; }

        /// <summary>
        /// Gets the current data context as a view model
        /// </summary>
        public T ViewModel => Element?.DataContext as T;

        private void Element_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            try
            {
                OnPropertyChanged(() => ViewModel);
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }
    }
}