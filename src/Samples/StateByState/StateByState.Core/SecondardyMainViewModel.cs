using System;
using BuildIt;
using BuildIt.Lifecycle.States.ViewModel;

namespace StateByState
{
    public class SecondardyMainViewModel:BaseViewModel
    {
        public event EventHandler Done;

        public string Data { get; set; }

        public SecondardyMainViewModel(ISpecial special)
        {
            Data = "Is special - " + special.Data;
        }

        public void IsDone()
        {
            Done.SafeRaise(this);

        }


    }
}