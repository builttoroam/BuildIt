using BuildIt;
using StateByState.Services;
using System;

namespace StateByState.Regions.Main.Third
{
    public class ThirdFourViewModel : NotifyBase
    {
        public ThirdFourViewModel(ISpecial special)
        {
            Title = special.Data;
        }

        public event EventHandler Done;

        public string Title { get; set; }

        public void SayImDone()
        {
            Done.SafeRaise(this);
        }
    }
}