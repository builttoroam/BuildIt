using System;
using BuildIt;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.States;
using StateByState.Services;

namespace StateByState
{
    public enum AdaptiveStates
    {
        Base,
        Normal,
        Wide
    }

    public class SecondardyMainViewModel:BaseStateManagerViewModelWithCompletion<DefaultCompletion>
    {
        private string data;
        public event EventHandler Done;

        public string Data
        {
            get { return data; }
            set
            {
                data = value;
                OnPropertyChanged();
            }
        }

        public SecondardyMainViewModel(ISpecial special)
        {
            StateManager
                .Group<AdaptiveStates>()
                .DefineState(AdaptiveStates.Normal)
                    .Target(this)
                        .Change(x => x.Data, (x, c) => x.Data = c)
                        .ToValue("Normal")
                .DefineState(AdaptiveStates.Wide)
                    .Target(this)
                        .Change(x => x.Data, (x, c) => x.Data= c)
                        .ToValue("Wide");





            Data = "Is special - " + special.Data;
        }

        public void IsDone()
        {
            OnComplete(DefaultCompletion.Complete);
            //Done.SafeRaise(this);

        }


    }
}