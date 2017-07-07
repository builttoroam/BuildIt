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
        /// Gets or sets the unique key used to identify this entity
        /// </summary>
        public Tuple<object, string> Key { get; set; }

        /// <summary>
        /// Gets or sets the element to adjust the property on
        /// </summary>
        public TElement Element { get; set; }

        /// <summary>
        /// Gets or sets the getter for the property
        /// </summary>
        public Func<TElement, TPropertyValue> Getter { get; set; }

        /// <summary>
        /// Gets or sets the setter for the property
        /// </summary>
        public Action<TElement, TPropertyValue> Setter { get; set; }

        /// <summary>
        /// Gets or sets the property to set
        /// </summary>
        public TPropertyValue Value { get; set; }

        private IDefaultValue Default =>
            new DefaultValue<TElement, TPropertyValue> { Element = Element, Setter = Setter, Value = Getter(Element) };

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