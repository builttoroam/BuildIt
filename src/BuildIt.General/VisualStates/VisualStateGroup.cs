using System;
using System.Collections.Generic;

namespace BuildIt.VisualStates
{
    public class VisualStateGroup<TVisualState> : IVisualStateGroup
        where TVisualState : struct
    {
        public IDictionary<TVisualState, IVisualState> VisualStates { get; } =
            new Dictionary<TVisualState, IVisualState>();

        private IDictionary<Tuple<object, string>, IDefaultValue> DefaultValues { get; } = new Dictionary<Tuple<object, string>, IDefaultValue>();

        private IVisualState VisualState<TFindVisualState>(TFindVisualState state) where TFindVisualState : struct
        {
            if (typeof(TFindVisualState) != typeof(TVisualState)) return null;
            var searchState = (TVisualState)(object)state;
            return VisualStates.SafeValue(searchState);
        }

        public void TransitionTo<TFindVisualState>(TFindVisualState state) where TFindVisualState : struct
        {
            var visualState = VisualState(state);
            if (visualState == null) return;
            visualState.TransitionTo(DefaultValues);
        }
    }
}