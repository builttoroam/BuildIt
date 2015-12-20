using System;
using System.Collections.Generic;

namespace BuildIt.VisualStates
{
    public interface IVisualState
    {
        void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues);
    }
}