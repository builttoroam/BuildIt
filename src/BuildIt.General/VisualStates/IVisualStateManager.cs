using System;
using System.Collections.Generic;

namespace BuildIt.VisualStates
{
    public interface IVisualStateManager
    {
        IDictionary<Type, IVisualStateGroup> VisualStateGroups { get; }
        void GoToState<TVisualState>(TVisualState state, bool animate = true) where TVisualState : struct;
    }
}