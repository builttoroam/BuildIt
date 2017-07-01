using System;
using System.Collections.Generic;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    public class StateValue<TElement, TPropertyValue> : IStateValue

    {
        public Tuple<object, string> Key { get; set; }

        public TElement Element { get; set; }

        public Func<TElement, TPropertyValue> Getter { get; set; }

        public Action<TElement, TPropertyValue> Setter { get; set; }

        public TPropertyValue Value { get; set; }

        private IDefaultValue Default => 
            new DefaultValue<TElement, TPropertyValue> { Element = Element, Setter=Setter, Value = Getter(Element) };

        public void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues)
        {
            if (!defaultValues.ContainsKey(Key))
            {
                defaultValues[Key] = Default;
            }
            Setter(Element, Value);
        }

    }
}