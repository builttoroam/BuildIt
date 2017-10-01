using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.States.Completion;
using StateByState.Services;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace StateByState.Regions.Main
{
    public class MainViewModel : BaseViewModelWithCompletion<MainCompletion>,
        ICompletionWithData<MainCompletion, int>
    {
        public MainViewModel(ISpecial special)
        {
            Data = special.Data;
        }

        public event EventHandler Completed;

        public event EventHandler<CompletionWithDataEventArgs<MainCompletion, int>> CompleteWithData;

        public event EventHandler UnableToComplete;

        public string Data { get; set; }

        public int TickCount => (int)DateTime.Now.Ticks;

#pragma warning disable 1998 // So we can do async actions

        public void Fourth()
        {
            CompleteWithData?.Invoke(this, new CompletionWithDataEventArgs<MainCompletion, int> { Completion = MainCompletion.Page4, Data = TickCount });
        }

        public async Task Init()
#pragma warning restore 1998
        {
            Data += " Hello Page 1";
            Debug.WriteLine("Break");
        }

        public void Spawn()
        {
            OnComplete(MainCompletion.NewRegion);
            // SpawnNewRegion.SafeRaise(this);
        }

        public async Task StartViewModel(CancellationToken cancel)
        {
            await Task.Delay(20000, cancel);
        }

        public void Test()
        {
            OnComplete(MainCompletion.Page2);
            // Completed?.Invoke(this, EventArgs.Empty);
        }

        public void Three()
        {
            OnComplete(MainCompletion.Page3);
            UnableToComplete?.Invoke(this, EventArgs.Empty);
        }
    }
}