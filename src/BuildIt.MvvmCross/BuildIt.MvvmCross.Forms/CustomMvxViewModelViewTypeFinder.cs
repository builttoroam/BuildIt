using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BuildIt.MvvmCross.Forms
{
    public class CustomMvxViewModelViewTypeFinder : MvxViewModelViewTypeFinder
    {
        public CustomMvxViewModelViewTypeFinder(IMvxViewModelByNameLookup viewModelByNameLookup, IMvxNameMapping viewToViewModelNameMapping) : base(viewModelByNameLookup, viewToViewModelNameMapping)
        {
        }

        protected override bool CheckCandidateTypeIsAView(Type candidateType)
        {
            if (candidateType == null)
                return false;

            if (candidateType.GetTypeInfo().IsAbstract)
                return false;

            //if (!typeof(IMvxView).IsAssignableFrom(candidateType))
            //    return false;

            return true;
        }
    }
}
