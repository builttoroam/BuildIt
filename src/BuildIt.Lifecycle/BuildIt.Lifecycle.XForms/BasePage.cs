using Xamarin.Forms;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Base page to inherit from
    /// </summary>
    public class BasePage : ContentPage
    {
        // protected override bool OnBackButtonPressed()
        // {
        //    return base.OnBackButtonPressed();
        // }
    }

    // public interface ICodeBehindViewModel<TViewModel>
    // {
    //    Wrapper<TViewModel> Data { get; }
    // }

    // public class Wrapper<T> : NotifyBase
    // {
    //    private T viewModel;

    // public T ViewModel
    //    {
    //        get { return viewModel; }
    //        private set
    //        {
    //            viewModel = value;
    //            OnPropertyChanged();
    //        }
    //    }

    // public Wrapper(FrameworkElement element)
    //    {
    //        element.DataContextChanged += Element_DataContextChanged;
    //    }

    // private void Element_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    //    {
    //        if (args.NewValue is T)
    //        {
    //            ViewModel = (T)args.NewValue;
    //        }
    //        else
    //        {
    //            ViewModel = default(T);
    //        }
    //    }
    // }
}