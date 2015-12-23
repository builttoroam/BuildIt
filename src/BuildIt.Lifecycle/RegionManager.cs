using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Autofac;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;
using Microsoft.Practices.ServiceLocation;

namespace BuildIt.Lifecycle
{
    public class RegionManager:IRegionManager
    {
        public Action<IRegionManager,IApplicationRegion> RegionCreated { get; set; }
        public Action<IRegionManager, IApplicationRegion> RegionIsClosing { get; set; }

        protected IContainer DependencyContainer { get; private set; }

        private IDictionary<string, IApplicationRegion> Regions { get; }=new Dictionary<string, IApplicationRegion>();

        private IApplicationRegion PrimaryRegion { get; set; }

        private List<Type> RegionTypes { get; set; } =new List<Type>(); 

        public void DefineRegion<TRegion>() where TRegion : IApplicationRegion
        {
            RegionTypes.Add(typeof(TRegion));
        }

        public TRegion RegionByType<TRegion>() where TRegion : IApplicationRegion
        {
            return (TRegion)Regions.Values.FirstOrDefault(x => x.GetType() == typeof (TRegion));
        }

        public bool IsPrimaryRegion(IApplicationRegion region)
        {
            return region == PrimaryRegion;
        }

        public void RegisterDependencies(IContainer container)
        {
            DependencyContainer = container;
            var cb = new ContainerBuilder();
            foreach (var state in RegionTypes)
            {
                cb.RegisterType(state);
            }
            cb.Update(container);
        }

        public TRegion CreateRegion<TRegion>() where TRegion : IApplicationRegion
        {
            $"Creating instance of {typeof(TRegion).Name}".Log();
            var vm = ServiceLocator.Current.GetInstance<TRegion>();

            //var uiRequired = vm as IRequiresUIAccess;
            //if (uiRequired != null)
            //{
            //    uiRequired.UIContext.RunContext = UIContext.RunContext;
            //}

            "Registering dependencies".Log();
            (vm as ICanRegisterDependencies)?.RegisterDependencies(DependencyContainer);

            vm.RegionId = Guid.NewGuid().ToString();

            Regions[vm.RegionId] = vm;

            // The first region created should be the primary region for this region manager (often the app!)
            if (Regions.Count == 1)
            {
                PrimaryRegion = vm;
            }

            vm.CloseRegion += RegionClosing;
            RegionCreated?.Invoke(this,vm);
            return vm;
        }

        private void RegionClosing(object sender, EventArgs e)
        {
            RegionIsClosing?.Invoke(this, sender as IApplicationRegion);
        }

        //public UIExecutionContext UIContext { get; } = new UIExecutionContext();
    }
}