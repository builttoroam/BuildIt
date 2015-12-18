using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BuildIt;
using BuildIt.Lifecycle.States.ViewModel;
using StateByState.Services;

namespace StateByState
{
    public class MainViewModel : BaseViewModel
    {
        public event EventHandler Completed;
        public event EventHandler UnableToComplete;

        public event EventHandler SpawnNewRegion;

        public MainViewModel(ISpecial special)
        {
            Data = special.Data;
        }

        public string Data { get; set; }

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
            Completed?.Invoke(this, EventArgs.Empty);
        }

        public void Three()
        {
            UnableToComplete?.Invoke(this, EventArgs.Empty);
        }

        public void Spawn()
        {
            SpawnNewRegion.SafeRaise(this);

        }

    }
}