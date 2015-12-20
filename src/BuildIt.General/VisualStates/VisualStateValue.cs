using System;
using System.Collections.Generic;

namespace BuildIt.VisualStates
{
    public class VisualStateValue<TElement, TPropertyValue> : IVisualStateValue

    {
        public Tuple<object, string> Key { get; set; }

        public TElement Element { get; set; }

        public Func<TElement, TPropertyValue> Getter { get; set; }

        public Action<TElement, TPropertyValue> Setter { get; set; }

        public TPropertyValue StateValue { get; set; }

        private IDefaultValue Default => new DefaultValue<TElement, TPropertyValue> { VisualStateValue = this, Value = Getter(Element) };

        public void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues)
        {
            if (!defaultValues.ContainsKey(Key))
            {
                defaultValues[Key] = Default;
            }
            Setter(Element, StateValue);
        }
    }
}