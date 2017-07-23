using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BuildIt.Forms
{
    public static class VisualElementProperties
    {
        public static IDictionary<string, object> Setters = new Dictionary<string, object>
        {
            {"IsVisible",  new Action<VisualElement, bool>((VisualElement ve, bool isVisible) => ve.IsVisible=isVisible) }
        };

        internal static Action<TElement, TPropertyValue> Lookup<TElement, TPropertyValue>(string name)
        {
            var action = Utilities.SafeValue<string, object, Action<TElement, TPropertyValue>>(Setters, name);
            return action;
        }
    }
}