using System.Collections.Generic;
using System;

namespace BuildIt.UI
{
    public interface INavigationViewModel
    {
        void OnAppearing(object parameter = null);

        void OnLeaving();
    }

    public interface IApplicationWithMapping
    {
        IDictionary<Type, IEventMap[]> Maps { get; }
    }
}