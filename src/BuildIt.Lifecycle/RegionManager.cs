using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;
using Microsoft.Practices.ServiceLocation;

namespace BuildIt.Lifecycle
{
    public interface IRegionManager : ICanRegisterDependencies
    {
        event EventHandler<ParameterEventArgs<IApplicationRegion>> RegionCreated;
        event EventHandler<ParameterEventArgs<IApplicationRegion>> RegionIsClosing;

        void DefineRegion<TRegion>() where TRegion : IApplicationRegion;

        TRegion CreateRegion<TRegion>() where TRegion : IApplicationRegion;

        TRegion RegionByType<TRegion>() where TRegion : IApplicationRegion;
    }

    public class RegionManager:IRegionManager
    {
        public event EventHandler<ParameterEventArgs<IApplicationRegion>> RegionCreated;
        public event EventHandler<ParameterEventArgs<IApplicationRegion>> RegionIsClosing;

        protected IContainer DependencyContainer { get; private set; }

        private IDictionary<string, IApplicationRegion> Regions { get; }=new Dictionary<string, IApplicationRegion>();

        private List<Type> RegionTypes { get; set; } =new List<Type>(); 

        public void DefineRegion<TRegion>() where TRegion : IApplicationRegion
        {
            RegionTypes.Add(typeof(TRegion));
        }

        public TRegion RegionByType<TRegion>() where TRegion : IApplicationRegion
        {
            return (TRegion)Regions.Values.FirstOrDefault(x => x.GetType() == typeof (TRegion));
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

            "Registering dependencies".Log();
            (vm as ICanRegisterDependencies)?.RegisterDependencies(DependencyContainer);

            vm.RegionId = Guid.NewGuid().ToString();

            Regions[vm.RegionId] = vm;

            vm.CloseRegion += RegionClosing;
            RegionCreated.SafeRaise(this,vm as IApplicationRegion);
            return vm;
        }

        private void RegionClosing(object sender, EventArgs e)
        {
            RegionIsClosing.SafeRaise(this, sender as IApplicationRegion);
        }
    }
}