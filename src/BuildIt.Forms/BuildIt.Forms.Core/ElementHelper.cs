using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BuildIt.Forms
{
    /// <summary>
    /// Static helper class
    /// </summary>
    public static class ElementHelper
    {
        /// <summary>
        /// Retrieves the target element and property info for a state action
        /// </summary>
        /// <param name="element">The root element (to begin search for target element)</param>
        /// <param name="setter">The state action</param>
        /// <returns>Reference to the target element and property info</returns>
        public static Tuple<Element, PropertyInfo> FindByTarget(this Element element, TargettedStateAction setter)
        {
            Element setterTarget = null;
            string prop = null;
            if (setter.Element != null)
            {
                setterTarget = setter.Element;
                prop = setter.Property;
            }

            if (setterTarget == null)
            {
                if (string.IsNullOrWhiteSpace(setter?.Target))
                {
                    return null;
                }

                var target = setter.Target.Split('.');
                var name = Enumerable.FirstOrDefault<string>(target);
                prop = Enumerable.Skip<string>(target, 1).FirstOrDefault();
                setterTarget = element.FindByName<Element>(name);

                if (setterTarget == null && string.IsNullOrWhiteSpace(prop))
                {
                    setterTarget = element;
                    prop = name;
                }

                if (setterTarget == null)
                {
                    var cv = element as ContentView;
                    if (cv != null)
                    {
                        foreach (var child in cv.Children)
                        {
                            setterTarget = child.FindByName<Element>(name);
                            if (setterTarget != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (setterTarget == null)
            {
                return null;
            }

            var targetProp = prop != null ? setterTarget?.GetType()?.GetProperty(prop) : null;
            return new Tuple<Element, PropertyInfo>(setterTarget, targetProp);
        }

        public static void AddDesignAction(this ContentPage element, string actionTitle, Action action)
        {
            if (!Debugger.IsAttached)
            {
                return;
            }

            var dtc = (element?.Content as Grid)?.Children?.OfType<DesignTimeControl>().FirstOrDefault();
            if (dtc == null)
            {
                "Unable to register design action. Only applicable to Content Page".Log();
                return;
            }

            var di = dtc.BindingContext as DesignInfo;
            di?.AddDesignAction(actionTitle, action);
        }
    }
}