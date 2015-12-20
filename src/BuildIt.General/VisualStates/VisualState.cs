using System;
using System.Collections.Generic;

namespace BuildIt.VisualStates
{
    public class VisualState<TVisualState> : IVisualState
    {
        public VisualState(TVisualState state)
        {
            State = state;
        }
        public TVisualState State { get; set; }

        public IList<IVisualStateValue> Values { get; } = new List<IVisualStateValue>();
        public void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues)
        {
            var defaults = new Dictionary<Tuple<object, string>, IDefaultValue>(defaultValues);

            foreach (var visualStateValue in Values)
            {
                visualStateValue.TransitionTo(defaultValues);
                defaults.Remove(visualStateValue.Key);
            }

            foreach (var defValue in defaults)
            {
                defValue.Value.RevertToDefault();
            }
        }
    }
}