using System;
using Windows.UI.Xaml;

namespace BuildIt.General.UI
{
    public class ContextWrapper<T> : NotifyBase
        where T:class
    {
        private T viewModel;

        public T ViewModel
        {
            get { return viewModel; }
            private set
            {
                viewModel = value;
                OnPropertyChanged();
            }
        }

        public ContextWrapper(FrameworkElement element)
        {
            viewModel = element.DataContext as T;
            element.DataContextChanged += Element_DataContextChanged;
        }

        private void Element_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            try
            {
                ViewModel = args.NewValue as T;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}