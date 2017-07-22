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
        /// Gets or sets gets the identifier for the target to apply getters/setters to
        /// </summary>
        public string TargetId { get; set; }

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

        /// <summary>
        /// Performs the state transition
        /// </summary>
        /// <param name="targets">The set of target elements to use in state transition</param>
        /// <param name="defaultValues">The set of default values to apply if state doesn't define property value</param>
        public virtual void TransitionTo(IDictionary<string, object> targets, IDictionary<Tuple<object, string>, IDefaultValue> defaultValues)
        {
            var element = Element;
            if (element == null)
            {
                element = targets.SafeValue<string, object, TElement>(TargetId);
            }

            if (!defaultValues.ContainsKey(Key))
            {
                defaultValues[Key] = Default(element);
            }

            Setter(element, Value);
        }

        private IDefaultValue Default(TElement element) =>
            new DefaultValue<TElement, TPropertyValue> { Element = Element, TargetId = TargetId, Setter = Setter, Value = Getter(element) };
    }
}