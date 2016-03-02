using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.States;
using Microsoft.Practices.ServiceLocation;

namespace BuildIt.Lifecycle
{
    public class RegionManager:IRegionManager
    {
        public Action<IRegionManager,IApplicationRegion> RegionCreated { get; set; }
        public Action<IRegionManager, IApplicationRegion> RegionIsClosing { get; set; }

        protected IDependencyContainer DependencyContainer { get; private set; }

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

        public IApplicationRegion RegionById(string id)
        {
            return Regions.SafeValue(id);
        }

        public bool IsPrimaryRegion(IApplicationRegion region)
        {
            return region == PrimaryRegion;
        }

        public void RegisterDependencies(IDependencyContainer container)
        {
            DependencyContainer = container;
            //var cb = new ContainerBuilder();
            container.StartUpdate();
            foreach (var state in RegionTypes)
            {
                container.RegisterType(state);
                //cb.RegisterType(state);
            }
            container.EndUpdate();
            //cb.Update(container);
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
            (vm as IRegisterDependencies)?.RegisterDependencies(DependencyContainer);

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