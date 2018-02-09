using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace BuildIt.General.UI
{
    /// <summary>
    /// General helper class
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Inverts a visibility value
        /// </summary>
        /// <param name="visible">The value to invert</param>
        /// <returns>The inverted value</returns>
        public static Visibility Inverse(this Visibility visible)
        {
            return visible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Converts bool to visibility value
        /// </summary>
        /// <param name="isVisible">bool value to convert</param>
        /// <returns>The corresponding visibility value</returns>
        public static Visibility ToVisibility(this bool isVisible)
        {
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Returns the selected item, and optionally sets the selected item back to null
        /// </summary>
        /// <typeparam name="T">The type of the selected item</typeparam>
        /// <param name="sender">The list element</param>
        /// <param name="resetSelectedIndex">Whether selected item should be set to null</param>
        /// <returns>The selected item</returns>
        public static T SelectedItem<T>(this object sender, bool resetSelectedIndex = true)
            where T : class
        {
            T selected = null;
            var list = sender as Selector;
            if (list != null)
            {
                selected = list.SelectedItem as T;
            }

            if (selected == null)
            {
                return default(T);
            }

            if (resetSelectedIndex)
            {
#if !NETFX_CORE
                if (sender is LongListSelector)
                {
                    var lls = sender as LongListSelector;
                    lls.SelectedItem = null;
                }
                else
                {
                    list.SelectedIndex = -1;
                }
#else
                list.SelectedIndex = -1;
#endif
            }

            return selected;
        }

        /// <summary>
        /// Finds an element by name and type
        /// </summary>
        /// <typeparam name="T">The type of element to search for</typeparam>
        /// <param name="container">The root element to search from</param>
        /// <param name="name">The (optional) name of element to retrieve</param>
        /// <returns>The matching element</returns>
        public static T FindControlByType<T>(this DependencyObject container, string name = null)
            where T : DependencyObject
        {
            T foundControl = null;

            // for each child object in the container
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(container); i++)
            {
                // is the object of the type we are looking for?
                if (VisualTreeHelper.GetChild(container, i) is T && (VisualTreeHelper.GetChild(container, i).GetValue(FrameworkElement.NameProperty).Equals(name) || name == null))
                {
                    foundControl = (T)VisualTreeHelper.GetChild(container, i);
                    break;
                }

                // if not, does it have children?
                else if (VisualTreeHelper.GetChildrenCount(VisualTreeHelper.GetChild(container, i)) > 0)
                {
                    // recursively look at its children
                    foundControl = FindControlByType<T>(VisualTreeHelper.GetChild(container, i), name);
                    if (foundControl != null)
                    {
                        break;
                    }
                }
            }

            return foundControl;
        }

        /// <summary>
        /// Returns all descendents to a particular depth
        /// </summary>
        /// <param name="root">The root element</param>
        /// <param name="depth">The depth to return elements to</param>
        /// <returns>The collection of elements</returns>
        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root, int depth)
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                yield return child;
                if (depth > 0)
                {
                    foreach (var descendent in Descendents(child, --depth))
                    {
                        yield return descendent;
                    }
                }
            }
        }

        /// <summary>
        /// The descendents of an element
        /// </summary>
        /// <param name="root">The root element</param>
        /// <returns>The descendents</returns>
        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root)
        {
            return Descendents(root, int.MaxValue);
        }

        /// <summary>
        /// The ancestors of the element
        /// </summary>
        /// <param name="root">The start element</param>
        /// <returns>The ancestors</returns>
        public static IEnumerable<DependencyObject> Ancestors(this DependencyObject root)
        {
            DependencyObject current = VisualTreeHelper.GetParent(root);
            while (current != null)
            {
                yield return current;
                current = VisualTreeHelper.GetParent(current);
            }
        }
    }
}
