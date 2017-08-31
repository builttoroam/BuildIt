using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BuildIt;
using BuildIt.AR.FormsSamples.Core.Services.Interfaces;
using BuildIt.AR.FormsSamples.Models;
using MvvmCross.Core.ViewModels;

namespace BuildIt.AR.FormsSamples.Core.ViewModels
{
    public class HomeViewModel : MvxViewModel
    {
        private IAppService AppService { get; }
        public HomeViewModel(IAppService appService)
        {
            AppService = appService;
        }

        public ObservableCollection<Organization> Organizations { get; } = new ObservableCollection<Organization>();

        public override async void Start()
        {
            try
            {
                base.Start();
                var results = await AppService.GetOrganizations();
                if (results != null)
                {
                    Organizations.Replace(results);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}
