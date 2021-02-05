using BuildIt.States.Interfaces;
using System;
using System.Collections.Generic;

namespace BuildIt.States
{
    /// <summary>
    /// Default value for a property.
    /// </summary>
    /// <typeparam name="TElement">The element to set the property on.</typeparam>
    /// <typeparam name="TPropertyValue">The property to set.</typeparam>
    public class DefaultValue<TElement, TPropertyValue> : IDefaultValue
    {
        /// <summary>
        /// Gets or sets the element to set the property on.
        /// </summary>
        public TElement Element { get; set; }

        /// <summary>
        /// Gets or sets gets the identifier for the target to apply getters/setters to.
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// Gets or sets the property setter.
        /// </summary>
        public Action<TElement, TPropertyValue> Setter { get; set; }

        /// <summary>
        /// Gets or sets the default value of the property.
        /// </summary>
        public TPropertyValue Value { get; set; }

        /// <summary>
        /// Resets the property back to the default value.
        /// </summary>
        /// <param name="targets">The set of target elements to use in state transition.</param>
        public void RevertToDefault(IDictionary<string, object> targets)
        {
            var element = Element;
            if (element == null)
            {
                element = targets.SafeValue<string, object, TElement>(TargetId);
            }

            Setter(element, Value);
        }
    }
}