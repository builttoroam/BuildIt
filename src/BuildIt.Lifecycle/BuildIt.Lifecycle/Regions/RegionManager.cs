using BuildIt.Lifecycle.Interfaces;
using BuildIt.Lifecycle.States;
using BuildIt.ServiceLocation;
using BuildIt.States.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildIt.Lifecycle.Regions
{
    /// <summary>
    /// Multi-region manager
    /// </summary>
    public class RegionManager : IRegionManager
    {
        /// <summary>
        /// Event for when a region is created
        /// </summary>
        public event EventHandler<DualParameterEventArgs<IRegionManager, IApplicationRegion>> RegionCreated;

        /// <summary>
        /// Event for when a region is closed
        /// </summary>
        public event EventHandler<DualParameterEventArgs<IRegionManager, IApplicationRegion>> RegionIsClosed;

        /// <summary>
        /// Gets the currently regions
        /// </summary>
        public IEnumerable<IApplicationRegion> CurrentRegions => Regions.Values;

        /// <summary>
        /// Gets the DI container
        /// </summary>
        protected IDependencyContainer DependencyContainer { get; private set; }

        private IDictionary<string, IApplicationRegion> Regions { get; } = new Dictionary<string, IApplicationRegion>();

        private IApplicationRegion PrimaryRegion { get; set; }

        private List<Type> RegionTypes { get; } = new List<Type>();

        /// <summary>
        /// Defines a new type of region within an application
        /// </summary>
        /// <typeparam name="TRegion">The type of region to register</typeparam>
        public void DefineRegion<TRegion>()
            where TRegion : IApplicationRegion
        {
            RegionTypes.Add(typeof(TRegion));
        }

        /// <summary>
        /// Retrieves all regions of a particular type
        /// </summary>
        /// <typeparam name="TRegion">The type of region to retrieve</typeparam>
        /// <returns>The matching regions</returns>
        public IEnumerable<TRegion> RegionByType<TRegion>()
            where TRegion : IApplicationRegion
        {
            return Regions.Values.OfType<TRegion>();
        }

        /// <summary>
        /// Retrieves a region by its Id
        /// </summary>
        /// <param name="id">The Id of the region to retrieve</param>
        /// <returns>The matchin region, or null if not found</returns>
        public IApplicationRegion RegionById(string id)
        {
            return Regions.SafeValue(id);
        }

        /// <summary>
        /// Determines if a region is the primary region of the application
        /// </summary>
        /// <param name="region">The region to check</param>
        /// <returns>true if the region is the primary region of the application</returns>
        public bool IsPrimaryRegion(IApplicationRegion region)
        {
            return region == PrimaryRegion;
        }

        /// <summary>
        /// Registers dependencies, and of children
        /// </summary>
        /// <param name="container">The DI container</param>
        public void RegisterDependencies(IDependencyContainer container)
        {
            DependencyContainer = container;
            // var cb = new ContainerBuilder();
            container.StartUpdate();
            foreach (var state in RegionTypes)
            {
                container.RegisterType(state);
                // cb.RegisterType(state);
            }

            container.EndUpdate();
            // cb.Update(container);
        }

        /// <summary>
        /// Creates a region of a specific type
        /// </summary>
        /// <typeparam name="TRegion">The type of region to create</typeparam>
        /// <param name="newRegionId">Optional Id for the new region</param>
        /// <returns>The newly created region, or null if not able to create the region</returns>
        public TRegion CreateRegion<TRegion>(string newRegionId = null)
            where TRegion : IApplicationRegion
        {
            try
            {
                $"Creating instance of {typeof(TRegion).Name}".LogLifecycleInfo();
                var vm = ServiceLocator.Current.GetInstance<TRegion>();

                // var uiRequired = vm as IRequiresUIAccess;
                // if (uiRequired != null)
                // {
                //    uiRequired.UIContext.RunContext = UIContext.RunContext;
                // }
                "Registering dependencies".LogLifecycleInfo();
                (vm as IRegisterDependencies)?.RegisterDependencies(DependencyContainer);

                if (!string.IsNullOrWhiteSpace(newRegionId))
                {
                    vm.RegionId = newRegionId;
                }

                // vm.RegionId = Guid.NewGuid().ToString();
                Regions[vm.RegionId] = vm;

                // The first region created should be the primary region for this region manager (often the app!)
                if (Regions.Count == 1)
                {
                    PrimaryRegion = vm;
                }

                vm.CloseRegion += RegionClosing;
                RegionCreated?.Invoke(this, new DualParameterEventArgs<IRegionManager, IApplicationRegion>(this, vm));
                return vm;
            }
            catch (Exception ex)
            {
                ex.LogLifecycleException();
                return default(TRegion);
            }
        }

        private void RegionClosing(object sender, EventArgs e)
        {
            RegionIsClosed?.Invoke(this, new DualParameterEventArgs<IRegionManager, IApplicationRegion>(this, sender as IApplicationRegion));
        }

        // public UIExecutionContext UIContext { get; } = new UIExecutionContext();
    }
}