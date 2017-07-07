using System;
using System.Collections.Generic;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Wraps a property/value combination to be set when a state is made active
    /// </summary>
    /// <typeparam name="TElement">The type of element to set property on</typeparam>
    /// <typeparam name="TPropertyValue">The type of property to set</typeparam>
    public class StateValue<TElement, TPropertyValue> : IStateValue

    {
        /// <summary>
        /// The unique key used to identify this entity
        /// </summary>
        public Tuple<object, string> Key { get; set; }

        /// <summary>
        /// The element to adjust the property on
        /// </summary>
        public TElement Element { get; set; }

        /// <summary>
        /// The getter for the property
        /// </summary>
        public Func<TElement, TPropertyValue> Getter { get; set; }

        /// <summary>
        /// The setter for the property
        /// </summary>
        public Action<TElement, TPropertyValue> Setter { get; set; }

        /// <summary>
        /// The property to set
        /// </summary>
        public TPropertyValue Value { get; set; }

        private IDefaultValue Default => 
            new DefaultValue<TElement, TPropertyValue> { Element = Element, Setter=Setter, Value = Getter(Element) };

        /// <summary>
        /// Performs the state transition
        /// </summary>
        /// <param name="defaultValues"></param>
        public virtual void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues)
        {
            if (!defaultValues.ContainsKey(Key))
            {
                defaultValues[Key] = Default;
            }
            Setter(Element, Value);
        }

    }
}