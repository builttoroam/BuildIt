using System;
using BuildIt;
using StateByState.Services;

namespace StateByState.Regions.Main
{
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
            Utilities.SafeRaise(Done, this);
        }
    }
}