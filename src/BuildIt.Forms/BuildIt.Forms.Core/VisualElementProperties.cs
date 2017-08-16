using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BuildIt.Forms
{
    /// <summary>
    /// Reference class to aid with setting common properties
    /// </summary>
    public static class VisualElementProperties
    {
        private static IDictionary<string, object> Setters { get; } = new Dictionary<string, object>
        {
            { nameof(VisualElement.IsVisible),  new Action<VisualElement, bool>((ve, isVisible) => ve.IsVisible = isVisible) }
        };

        /// <summary>
        /// Looks up a property getter
        /// </summary>
        /// <typeparam name="TElement">The element type to retrieve</typeparam>
        /// <typeparam name="TPropertyValue">The property type</typeparam>
        /// <param name="name">The name of the property</param>
        /// <returns>The action to retrieve the property value</returns>
        public static Action<TElement, TPropertyValue> Lookup<TElement, TPropertyValue>(string name)
        {
            var action = Setters.SafeValue<string, object, Action<TElement, TPropertyValue>>(name);
            return action;
        }
    }
}