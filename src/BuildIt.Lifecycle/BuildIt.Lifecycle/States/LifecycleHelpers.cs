using System;
using System.ComponentModel;
using BuildIt.Lifecycle.Interfaces;
using BuildIt.States;
using BuildIt.States.Completion;
using BuildIt.States.Interfaces.Builder;

namespace BuildIt.Lifecycle.States
{
    public static class LifecycleHelpers
    {
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
            //if (smInfo?.Item1 == null ||
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

            var returnd = smreturn.WhenChangedTo((vm,cancel) =>
            {
                vm.Complete += changeStateAction;
            })
                .WhenChangingFrom((vm, cancel) =>
                {
                    vm.Complete -= changeStateAction;
                });
            return returnd;
            //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        public static
            IStateDefinitionWithDataBuilder<TState, TStateData>

            LaunchRegion<TState, TStateData, TCompletion, TNewRegion>(
            this IStateWithDataCompletionBuilder<TState, TStateData, TCompletion> smInfo,
            IApplicationRegion region, TypeRef.TypeWrapper<TNewRegion> wrapper)
            where TState : struct
            where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
            where TCompletion : struct
            where TNewRegion : IApplicationRegion
        {
            //if (smInfo?.Item1 == null ||
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
                .WhenChangingFrom( (vm, cancel) =>
                {
                    vm.Complete -= changeStateAction;
                });
            return returnd;
            //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

    }
}
