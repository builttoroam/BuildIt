using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace BuildIt
{
    /// <summary>
    /// Helper for building information about a Type
    /// </summary>
    public class TypeHelper
    {
        private Func<object, int> hashBuilder;

        private Action<object, object> deepUpdater;

        /// <summary>
        /// Defines an entity with an Updater method
        /// </summary>
        private interface ICollectionUpdater
        {
            /// <summary>
            /// Gets method that is used to update the collection
            /// </summary>
            Action Updater { get; }
        }

        /// <summary>
        /// Gets or sets gets the function for building a hash for entities of this type
        /// </summary>
        public Func<object, int> HashBuilder { get => hashBuilder ?? (hashBuilder = CreateHashBuilder()); set => hashBuilder = value; }

        /// <summary>
        /// Gets or sets gets the function for updating entities of this type
        /// </summary>
        public Action<object, object> DeepUpdater { get => deepUpdater ?? (deepUpdater = CreateDeepUpdater()); set => deepUpdater = value; }

        private static IDictionary<Type, TypeHelper> Helpers { get; } = new Dictionary<Type, TypeHelper>();

        /// <summary>
        /// Gets or sets the type to provide helpers for
        /// </summary>
        private Type TypeToHelp { get; set; }

        /// <summary>
        /// Generates a type helper for a specific type
        /// </summary>
        /// <param name="typeToHelp">The type to provide help for</param>
        /// <returns>An instance of the TypeHelper</returns>
        public static TypeHelper RetrieveHelperForType(Type typeToHelp)
        {
            var existing = Helpers.SafeValue(typeToHelp);
            if (existing == null)
            {
                existing = new TypeHelper { TypeToHelp = typeToHelp };
                Helpers[typeToHelp] = existing;
            }

            return existing;
        }

        private Func<object, int> CreateHashBuilder()
        {
            if (hashBuilder == null)
            {
                SetupTypeHelper();
            }

            return hashBuilder;
        }

        private Action<object, object> CreateDeepUpdater()
        {
            if (deepUpdater == null)
            {
                SetupTypeHelper();
            }

            return deepUpdater;
        }

        private void CollectionUpdater(object obj1, object obj2)
        {
            var type = obj1.GetType().GenericTypeArguments;
            if (type == null)
            {
                return;
            }

            var updaterType = typeof(TypedCollectionUpdater<>).MakeGenericType(type);
            var updater = Activator.CreateInstance(updaterType, new object[] { obj1, obj2 }) as ICollectionUpdater;
            updater?.Updater();
        }

        private void SetupTypeHelper()
        {
            var hashBuilder = new Func<object, int>(obj => 0);
            var updater = new Action<object, object>((obj1, obj2) => { });

            if (TypeToHelp.GetTypeInfo().ImplementedInterfaces.Contains(typeof(INotifyCollectionChanged)))
            {
                this.hashBuilder = hashBuilder;
                deepUpdater = CollectionUpdater;
                return;
            }

            var valProperties = TypeToHelp.GetRuntimeProperties().Where(x => x.PropertyType.GetTypeInfo().IsValueType || x.PropertyType == typeof(string));
            foreach (var prop in valProperties)
            {
                var oldBuilder = hashBuilder;
                hashBuilder = (object obj) => oldBuilder(obj).CombineHashCode(prop.GetValue(obj)?.GetHashCode() ?? 0);
                var oldUpdater = updater;
                updater = (object obj1, object obj2) =>
                {
                    oldUpdater(obj1, obj2);
                    var obj1Value = prop.GetValue(obj1);
                    var obj2Value = prop.GetValue(obj2);
                    if (obj1Value != obj2Value)
                    {
                        prop.SetValue(obj1, obj2Value);
                        var changeUpdater = obj1 as IRaisePropertyChanged;
                        if (changeUpdater != null)
                        {
                            changeUpdater.RaisePropertyChanged(prop.Name);
                        }
                    }
                };
            }

            this.hashBuilder = hashBuilder;

            var valUpdater = updater;

            var objectProperties = TypeToHelp.GetRuntimeProperties().Where(x => !x.PropertyType.GetTypeInfo().IsValueType && x.PropertyType != typeof(string));
            var objectUpdater = new Action<object, object>((obj1, obj2) => { });
            foreach (var prop in objectProperties)
            {
                var typeHelper = RetrieveHelperForType(prop.PropertyType);
                var oldUpdater = objectUpdater;
                objectUpdater = (object obj1, object obj2) =>
                {
                    oldUpdater(obj1, obj2);

                    var changeUpdater = obj1 as IRaisePropertyChanged;

                    var obj1Value = prop.GetValue(obj1);
                    var obj2Value = prop.GetValue(obj2);
                    if (obj1Value == null)
                    {
                        if (obj2Value != null)
                        {
                            prop.SetValue(obj1, obj2Value);
                            if (changeUpdater != null)
                            {
                                changeUpdater.RaisePropertyChanged(prop.Name);
                            }
                        }
                    }
                    else
                    {
                        if (obj2Value == null)
                        {
                            prop.SetValue(obj1, obj2Value);
                            if (changeUpdater != null)
                            {
                                changeUpdater.RaisePropertyChanged(prop.Name);
                            }
                        }
                        else
                        {
                            typeHelper.DeepUpdater(obj1Value, obj2Value);
                        }
                    }
                };
            }

            updater = (object obj1, object obj2) =>
            {
                if (obj1 == null || obj2 == null)
                {
                    return;
                }

                var obj1Hash = hashBuilder(obj1);
                var obj2Hash = hashBuilder(obj2);
                if (obj1Hash != obj2Hash)
                {
                    valUpdater(obj1, obj2);
                }

                objectUpdater(obj1, obj2);
            };
            deepUpdater = updater;
        }

        private class TypedCollectionUpdater<T> : ICollectionUpdater
        {
            private Action<object, object> deepUpdater;

            /// <summary>
            /// Initializes a new instance of the <see cref="TypedCollectionUpdater{T}"/> class.
            /// </summary>
            /// <param name="list1">The original list</param>
            /// <param name="list2">The new list</param>
            public TypedCollectionUpdater(object list1, object list2)
            {
                List1 = list1 as ObservableCollection<T>;
                List2 = list2 as ObservableCollection<T>;
            }

            public Action Updater => Update;

            private Action<object, object> DeepUpdater => deepUpdater ?? (deepUpdater = TypeHelper.RetrieveHelperForType(typeof(T)).DeepUpdater);

            private ObservableCollection<T> List1 { get; }

            private ObservableCollection<T> List2 { get; }

            private void Update()
            {
                for (int i = 0; i < List1.Count; i++)
                {
                    var list1Item = List1[i];
                    if (i >= List2.Count)
                    {
                        List1.RemoveAt(i);
                    }
                    else
                    {
                        DeepUpdater(list1Item, List2[i]);
                    }
                }

                while (List1.Count < List2.Count)
                {
                    List1.Add(List2[List1.Count]);
                }
            }
        }
    }
}