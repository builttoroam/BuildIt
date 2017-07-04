using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BuildIt.Forms.Core
{
    public static class ElementHelper
    {
        public static Tuple<Element, PropertyInfo> FindByTarget(this Element element, TargettedStateAction setter)
        {
            if (string.IsNullOrWhiteSpace(setter?.Target)) return null;
            //var setterTarget
            var target = setter.Target.Split('.');
            var name = Enumerable.FirstOrDefault<string>(target);
            var prop = Enumerable.Skip<string>(target, 1).FirstOrDefault();
            var setterTarget = element.FindByName<Element>(name);
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
            var targetProp = prop != null ? setterTarget?.GetType()?.GetProperty(prop) : null;
            if (setterTarget == null) return null;
            return new Tuple<Element, PropertyInfo>(setterTarget, targetProp);
        }
    }
}