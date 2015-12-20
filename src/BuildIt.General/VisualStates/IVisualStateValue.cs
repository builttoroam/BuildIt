using System;
using System.Collections.Generic;

namespace BuildIt.VisualStates
{
    public interface IVisualStateValue
    {
        Tuple<object, string> Key { get; }
        void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues);
    }
}