using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Forms.Platform;
using MvvmCross.Forms.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BuildIt.MvvmCross.Forms
{
    public class CustomMvxFormsPagePresenter : MvxFormsPagePresenter
    {
        public CustomMvxFormsPagePresenter(MvxFormsApplication formsApplication, IMvxViewsContainer viewsContainer = null, IMvxViewModelTypeFinder viewModelTypeFinder = null, IMvxViewModelLoader viewModelLoader = null, Dictionary<Type, MvxPresentationAttributeAction> attributeTypesToActionsDictionary = null) : base(formsApplication, viewsContainer, viewModelTypeFinder, viewModelLoader, attributeTypesToActionsDictionary)
        {
        }

        public override Page CreatePage(Type viewType, MvxViewModelRequest request, MvxBasePresentationAttribute attribute)
        {
            var page = base.CreatePage(viewType, request, attribute);
            if (page!=null && page.BindingContext == null)
            { 
                if (request is MvxViewModelInstanceRequest instanceRequest)
                    page.BindingContext= instanceRequest.ViewModelInstance;
                else
                    page.BindingContext = ViewModelLoader.LoadViewModel(request, null);
            }

            return page;
        }
    }
}
