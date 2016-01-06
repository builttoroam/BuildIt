﻿using System;
using System.Threading.Tasks;
using Autofac;
using BuildIt;
using BuildIt.Lifecycle;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.States;
using StateByState.Services;

namespace StateByState
{
    public enum ThirdStates
    {
        Base,
        One,
        Two,
        Three,
        Four
    }

    public enum ThirdTransitions
    {
        Base,
        //OneToTwo,
        //TwoToThree,
        //ThreeToFour,
        //FourToOne
    }

    public class ThirdViewModel:BaseStateManagerViewModel
    {
        public event EventHandler ThirdCompleted;

        public ThirdViewModel()
        {

            var sm = StateManager;
            var gp = sm.GroupWithViewModels<ThirdStates>().Item2;
            gp.DefineViewModelState<ThirdOneViewModel>(ThirdStates.One);
            gp.DefineViewModelState<ThirdTwoViewModel>(ThirdStates.Two);
            gp.DefineViewModelState<ThirdThreViewModel>(ThirdStates.Three);
            gp.DefineViewModelState<ThirdFourViewModel>(ThirdStates.Four)
                .WhenChangedTo(vm =>
                {
                    vm.Done += Vm_Done;
                }).
                WhenChangingFrom(vm =>
                {
                    vm.Done -= Vm_Done;
                });
            
        }

        //public override void RegisterDependencies(IContainer container)
        //{
        //    base.RegisterDependencies(container);

        //    (StateManager as ICanRegisterDependencies)?.RegisterDependencies(container);
        //}


        public async Task Start()
        {
            await StateManager.GoToState(ThirdStates.One);
        }

        public async void One()
        {
            await StateManager.GoToState(ThirdStates.Two);// ThirdTransitions.OneToTwo);
        }

        public async void Two ()
        {
            await StateManager.GoToState(ThirdStates.Three);// ThirdTransitions.TwoToThree);

        }

        public async void Three()
        {

            await StateManager.GoToState(ThirdStates.Four);// ThirdTransitions.ThreeToFour);
        }
        public async void Four()
        {
            await StateManager.GoToState(ThirdStates.One);// ThirdTransitions.FourToOne);

        }
    }

        public class ThirdOneViewModel : NotifyBase { public string Title => "One"; }
    public class ThirdTwoViewModel : NotifyBase { public string Title => "Two"; }
    public class ThirdThreViewModel : NotifyBase { public string Title => "Three"; }

    public class ThirdFourViewModel : NotifyBase
    {
        public event EventHandler Done;
        public string Title { get; set; }

        public ThirdFourViewModel(ISpecial special)
        {
            Title = special.Data;
        }

        public void SayImDone()
        {
            Done.SafeRaise(this);
        }
    }
}
