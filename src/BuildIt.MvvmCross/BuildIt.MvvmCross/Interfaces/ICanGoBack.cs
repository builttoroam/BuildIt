using System;
using System.Threading.Tasks;

namespace BuildIt.MvvmCross.Interfaces
{
    public interface ICanGoBack
    {
        event EventHandler ClearPreviousViews;

        // TODO: remove this method
        Task GoingBack(CancelEventArgs e);
    }
}