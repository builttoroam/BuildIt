using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;


namespace BuildIt.VisualStates
{
    public class VisualStateManager : IVisualStateManager
    {
        public IDictionary<Type, IVisualStateGroup> VisualStateGroups { get; } =
            new Dictionary<Type, IVisualStateGroup>();

        public void GoToState<TVisualState>(TVisualState state, bool animate = true) where TVisualState : struct
        {
            IVisualStateGroup group = VisualStateGroups.SafeValue(state.GetType());
            if (group == null) return;
            group.TransitionTo(state);
        }
    }
}