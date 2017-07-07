using System;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Default value for a property
    /// </summary>
    /// <typeparam name="TElement">The element to set the property on</typeparam>
    /// <typeparam name="TPropertyValue">The property to set</typeparam>
    public class DefaultValue<TElement, TPropertyValue> : IDefaultValue
    {
        /// <summary>
        /// Gets or sets the element to set the property on
        /// </summary>
        public TElement Element { get; set; }

        /// <summary>
        /// Gets or sets the property setter
        /// </summary>
        public Action<TElement, TPropertyValue> Setter { get; set; }

        /// <summary>
        /// Gets or sets the default value of the property
        /// </summary>
        public TPropertyValue Value { get; set; }

        /// <summary>
        /// Resets the property back to the default value
        /// </summary>
        public void RevertToDefault()
        {
            Setter(Element, Value);
        }
    }
}