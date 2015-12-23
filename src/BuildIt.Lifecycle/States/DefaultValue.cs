using System;

namespace BuildIt.Lifecycle.States
{
    public class DefaultValue<TElement, TPropertyValue> : IDefaultValue

    {
        public TElement Element { get; set; }
        public Action<TElement, TPropertyValue> Setter { get; set; }

        public TPropertyValue Value { get; set; }

        public void RevertToDefault()
        {
            Setter(Element,Value);
        }
    }
}