using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildIt;
using BuildIt.States;

namespace States.Sample.Core
{
    public enum LoadingStates
    {
        Base,
        Loading,
        Loaded,
        LoadingFailed
    }

    public enum SizeStates
    {
        Base,
        Narrow,
        Normal,
        Large
    }

    public class MainViewModel:NotifyBase,IHasStates
    {
        private string currentStateName = "Test data";
        public IStateManager StateManager { get; } = new StateManager();

        public string CurrentStateName
        {
            get { return currentStateName; }
            set
            {
                currentStateName = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            StateManager.Group<LoadingStates>().DefineAllStates()
                .Group<SizeStates>().DefineAllStates();

            StateManager.Group<SizeStates>()
                .DefineState(SizeStates.Narrow)
                .ChangePropertyValue(vm => CurrentStateName, "Narrow")
                .DefineState(SizeStates.Normal)
                .ChangePropertyValue(vm => CurrentStateName, "Normal")
                .DefineState(SizeStates.Large)
                .ChangePropertyValue(vm => CurrentStateName, "Large");
        }
    }
}
