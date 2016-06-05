using System;
using Windows.UI.Xaml;

namespace BuildIt.General.UI
{
    public class ContextWrapper<T> : NotifyBase
        where T:class
    {
        public FrameworkElement Element { get; set; }

        public T ViewModel => Element?.DataContext as T;

        public ContextWrapper(FrameworkElement element)
        {
            Element = element;
            element.DataContextChanged += Element_DataContextChanged;
        }

        private void Element_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            try
            {
                OnPropertyChanged(()=>ViewModel);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}