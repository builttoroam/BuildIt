using System;

namespace BuildIt.UI
{
    public class EventMap<TViewModel, TDelegate> : IEventMap
    {
        public EventMap(TDelegate action, Action<TViewModel, TDelegate> arrive, Action<TViewModel, TDelegate> leave)
        {
            Action = action;
            Arrive = arrive;
            Leave = leave;
        }

        public TDelegate Action { get; set; }

        public Action<TViewModel, TDelegate> Arrive { get; set; }

        public Action<TViewModel, TDelegate> Leave { get; set; }

        public void Wire(object viewModel)
        {
            Arrive((TViewModel)viewModel, Action);
        }

        public void Unwire(object viewModel)
        {
            Leave((TViewModel)viewModel, Action);
        }
    }
}