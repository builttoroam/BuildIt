using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BuildIt;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.States.Completion;
using StateByState.Services;

namespace StateByState
{
    public enum MainCompletion
    {
        Base,
        Page2,
        Page3,
        Page4,
        NewRegion
    }

    public class MainViewModel : BaseViewModelWithCompletion<MainCompletion>, 
        ICompletionWithData<MainCompletion, int>
    {
        public event EventHandler Completed;
        public event EventHandler UnableToComplete;

        //public event EventHandler SpawnNewRegion;

        public MainViewModel(ISpecial special)
        {
            Data = special.Data;
        }

        public string Data { get; set; }

        public int TickCount => (int) DateTime.Now.Ticks;

#pragma warning disable 1998 // So we can do async actions
        public async Task Init()
#pragma warning restore 1998
        {
            

            Data += " Hello Page 1";
            Debug.WriteLine("Break");
        }

        public async Task StartViewModel()
        {
            await Task.Delay(20000);
        }

        public void Test()
        {
            OnComplete(MainCompletion.Page2);
            Completed?.Invoke(this, EventArgs.Empty);
        }

        public void Three()
        {
            OnComplete(MainCompletion.Page3);
            UnableToComplete?.Invoke(this, EventArgs.Empty);
        }

        public void Spawn()
        {
            OnComplete(MainCompletion.NewRegion);
            //SpawnNewRegion.SafeRaise(this);

        }

        public void Fourth()
        {
            CompleteWithData?.Invoke(this,new CompletionWithDataEventArgs<MainCompletion, int> {Completion = MainCompletion.Page4,Data=TickCount});
        }

        public event EventHandler<CompletionWithDataEventArgs<MainCompletion, int>> CompleteWithData;
    }
}