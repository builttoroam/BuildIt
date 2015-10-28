using System.Collections.Generic;
#if NETFX_CORE
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Phone.Controls;
#endif

namespace BuiltToRoam.UI
{
    public static class Utilities
    {
        public static Visibility Inverse(this Visibility visible)
        {
            return visible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public static Visibility ToVisibility(this bool isVisible)
        {
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public static T SelectedItem<T>(this object sender, bool resetSelectedIndex = true) where T : class
        {
            T selected = null;
            var list = sender as Selector;
            if (list == null)
            {
#if !NETFX_CORE
                var lls = sender as LongListSelector;
                if (lls == null)
                {
                    var pvt = sender as Pivot;
                    if (pvt == null) return default(T);
                    selected = pvt.SelectedItem as T;
                    resetSelectedIndex = false;
                    
                }
                else
                {
                    selected = lls.SelectedItem as T;
                }
#endif


            }
            else
            {
                selected = list.SelectedItem as T;
            }
            if (selected == null) return default(T);
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


        public static T FindControlByType<T>(this DependencyObject container, string name = null) where T : DependencyObject
        {
            T foundControl = null;

            //for each child object in the container
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(container); i++)
            {
                //is the object of the type we are looking for?
                if (VisualTreeHelper.GetChild(container, i) is T && (VisualTreeHelper.GetChild(container, i).GetValue(FrameworkElement.NameProperty).Equals(name) || name == null))
                {
                    foundControl = (T)VisualTreeHelper.GetChild(container, i);
                    break;
                }
                //if not, does it have children?
                else if (VisualTreeHelper.GetChildrenCount(VisualTreeHelper.GetChild(container, i)) > 0)
                {
                    //recursively look at its children
                    foundControl = FindControlByType<T>(VisualTreeHelper.GetChild(container, i), name);
                    if (foundControl != null)
                        break;
                }
            }

            return foundControl;
        }

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
                        yield return descendent;
                }
            }
        }

        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root)
        {
            return Descendents(root, int.MaxValue);
        }

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
