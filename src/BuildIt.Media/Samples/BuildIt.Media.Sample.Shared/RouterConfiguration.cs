using System;
using System.Collections.Generic;
using BuildIt.Media.Sample.Views;
using BuildIt.Media.Sample.ViewModels;
using Uno.Extensions.Navigation;

namespace BuildIt.Media.Sample
{
    public class RouterConfiguration : IRouteDefinitions
    {
        public IReadOnlyDictionary<string, (Type, Type)> Routes { get; } = new Dictionary<string, (Type, Type)>()
            .RegisterPage<MainPageViewModel, MainPage>(string.Empty)
            .RegisterPage<SecondPageViewModel, SecondPage>();
    }
}
