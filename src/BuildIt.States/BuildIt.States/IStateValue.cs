using System;
using System.Collections.Generic;

namespace BuildIt.States
{
    public interface IStateValue
    {
        Tuple<object, string> Key { get; }

        void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues);
    }
}