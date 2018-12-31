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
        private static Assembly FormsAssemblyForLogging { get; } = typeof(ElementHelper).GetTypeInfo().Assembly;

        /// <summary>
        /// Applies an action to all nested children of an element
        /// </summary>
        /// <typeparam name="TElement">The minimum level base class that is necessary for the child for the action to be performed</typeparam>
        /// <param name="view">The target element</param>
        /// <param name="action">The action to perform</param>
        /// <param name="applyToRoot">Apply this action to the target element</param>
        public static void ApplyToAllNested<TElement>(Element view, Action<TElement> action, bool applyToRoot)
            where TElement : View
        {
            if (view == null)
            {
                "null".LogFormsInfo();
                return;
            }

            $"attempting to match {view.GetType().Name}".LogFormsInfo();

            if (view is TElement element && applyToRoot)
            {
                "matching view found".LogFormsInfo();
                action(element);
            }

            if (view is Layout layout)
            {
                $"searching children of {view.GetType().Name}".LogFormsInfo();
                foreach (var subelement in layout.Children)
                {
                    ApplyToAllNested(subelement, action, true);
                }

                layout.ChildAdded += (s, e) =>
                {
                    "child added".LogFormsInfo();
                    ApplyToAllNested(e.Element, action, true);
                };
            }
        }

        /// <summary>
        /// Adds an action that can be run from the design overlay
        /// </summary>
        /// <param name="element">The page or user control</param>
        /// <param name="actionTitle">The text that will appear on screen</param>
        /// <param name="action">The action to be performed</param>
        public static void AddDesignAction(this ContentPage element, string actionTitle, Action action)
        {
            if (!Debugger.IsAttached)
            {
                return;
            }

            var dtc = (element?.Content as Grid)?.Children?.OfType<DesignTimeControl>().FirstOrDefault();
            if (dtc == null)
            {
                "Unable to register design action. Only applicable to Content Page".LogFormsInfo();
                return;
            }

            var di = dtc.BindingContext as DesignInfo;
            di?.AddDesignAction(actionTitle, action);
        }

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
                    if (element is ContentView cv)
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

        /// <summary>
        /// Quick log for states messages
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="message">The message to log</param>
        internal static void LogFormsException(this Exception exception, string message = null)
        {
            exception.LogError(assembly: FormsAssemblyForLogging, message: message);
        }

        /// <summary>
        /// Quick log for states messages
        /// </summary>
        /// <param name="message">The message to log</param>
        internal static void LogFormsInfo(this string message)
        {
            message.LogMessage(assembly: FormsAssemblyForLogging);
        }
    }

    //public static class SafeNameScopeExtensions
    //{
    //    /// <typeparam name="T">The type of instance to find.</typeparam>
    //    /// <param name="element">An element in the scope to search.</param>
    //    /// <param name="name">The name of the element to find.</param>
    //    /// <summary>Returns the instance of type <paramref name="T" /> that has name <paramref name="name" /> in the scope that includes <paramref name="element" />.</summary>
    //    /// <returns>To be added.</returns>
    //    /// <remarks>To be added.</remarks>
    //    public static T FindByName<T>(this Element element, string name)
    //    {
    //        try
    //        {
    //            return NameScopeExtensions.FindByName<T>(element, name);
    //        }
    //        catch (Exception ex)
    //        {
    //            ex.LogError();
    //        }

    //        try
    //        {
    //            var child = (from e in element.Descendants().OfType<VisualElement>()
    //                         where e.GetValue(Namepr))
    //        }
    //        catch (Exception ex)
    //        {
    //            ex.LogError();
    //        }

    //        return default;
    //    }
    //}
}