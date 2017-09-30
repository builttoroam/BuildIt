using BuildIt.Lifecycle.Interfaces;
using BuildIt.States;
using BuildIt.States.Completion;
using BuildIt.States.Interfaces.Builder;
using System;
using System.ComponentModel;
using System.Reflection;

namespace BuildIt.Lifecycle.States
{
    /// <summary>
    /// Helpers for working with Lifecycle
    /// </summary>
    public static class LifecycleHelpers
    {
        private static Assembly LifecycleAssemblyForLogging { get; } = typeof(LifecycleHelpers).GetTypeInfo().Assembly;

        /// <summary>
        /// Closes a region
        /// </summary>
        /// <typeparam name="TState">The type of state</typeparam>
        /// <typeparam name="TStateData">The type of state data</typeparam>
        /// <typeparam name="TCompletion">The type of completion</typeparam>
        /// <param name="smInfo">The state builder</param>
        /// <param name="region">The region where the states are defined</param>
        /// <returns>Updated state builder</returns>
        public static

                    IStateDefinitionWithDataBuilder<TState, TStateData>
                    CloseRegion<TState, TStateData, TCompletion>(
            this
                    IStateWithDataCompletionBuilder<TState, TStateData, TCompletion> smInfo,
            IApplicationRegion region)
            where TState : struct
            where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
            where TCompletion : struct
        {
            // if (smInfo?.Item1 == null ||
            //    smInfo.Item2 == null ||
            //    smInfo.Item3 == null) return null;
            var comp = smInfo?.Completion;
            var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>(async (s, e) =>
            {
                if (e.Completion.Equals(comp))
                {
                    await region.RequestClose();
                }
            });

            var smreturn = smInfo;

            var returnd = smreturn.WhenChangedTo((vm, cancel) =>
            {
                vm.Complete += changeStateAction;
            })
                .WhenChangingFrom((vm, cancel) =>
                {
                    vm.Complete -= changeStateAction;
                });
            return returnd;
            // return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        /// <summary>
        /// Launches a region when a state raises a completion event
        /// </summary>
        /// <typeparam name="TState">The type of state</typeparam>
        /// <typeparam name="TStateData">The type of state data to be passed to the new region</typeparam> TODO: Pass data to new region
        /// <typeparam name="TCompletion">The type of completion event</typeparam>
        /// <typeparam name="TNewRegion">The type of new region to create</typeparam>
        /// <param name="smInfo">The state builder</param>
        /// <param name="region">The region where the states are defined</param>
        /// <param name="wrapper">The type reference to the new region - use TypeRef.Get to get type reference</param>
        /// <returns>Updated state builder</returns>
        public static
                    IStateDefinitionWithDataBuilder<TState, TStateData>
                    LaunchRegion<TState, TStateData, TCompletion, TNewRegion>(
                    this IStateWithDataCompletionBuilder<TState, TStateData, TCompletion> smInfo,
                    IApplicationRegion region,
                    TypeRef.TypeWrapper<TNewRegion> wrapper)
                    where TState : struct
                    where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
                    where TCompletion : struct
                    where TNewRegion : IApplicationRegion
        {
            // if (smInfo?.Item1 == null ||
            //    smInfo.Item2 == null ||
            //    smInfo.Item3 == null) return null;
            var comp = smInfo?.Completion;
            var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>((s, e) =>
            {
                if (e.Completion.Equals(comp))
                {
                    region.Manager.CreateRegion<TNewRegion>();
                }
            });

            var smreturn = smInfo;

            var returnd = smreturn.WhenChangedTo((vm, cancel) =>
            {
                vm.Complete += changeStateAction;
            })
                .WhenChangingFrom((vm, cancel) =>
               {
                   vm.Complete -= changeStateAction;
               });
            return returnd;
            // return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        /// <summary>
        /// Quick log for lifecycle messages
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="message">The message to log</param>
        internal static void LogLifecycleException(this Exception exception, string message = null)
        {
            exception.LogError(assembly: LifecycleAssemblyForLogging, message: message);
        }

        /// <summary>
        /// Quick log for lifecycle messages
        /// </summary>
        /// <param name="message">The message to log</param>
        internal static void LogLifecycleInfo(this string message)
        {
            message.LogMessage(assembly: LifecycleAssemblyForLogging);
        }
    }
}