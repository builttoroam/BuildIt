using System;
using BuildIt;
using BuildIt.Lifecycle.States.ViewModel;
using StateByState.Services;

namespace StateByState
{
    public class SecondardyMainViewModel:BaseViewModelWithCompletion<DefaultCompletion>
    {
        public event EventHandler Done;

        public string Data { get; set; }

        public SecondardyMainViewModel(ISpecial special)
        {
            Data = "Is special - " + special.Data;
        }

        public void IsDone()
        {
            OnComplete(DefaultCompletion.Complete);
            //Done.SafeRaise(this);

        }


    }
}