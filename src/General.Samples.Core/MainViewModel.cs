using BuildIt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace General.Samples.Core
{

    public class MainViewModel : NotifyBase, IViewModelWithState<Person>
    {
        private Person _state;

        public Person State { get => _state; set => SetProperty(ref _state, value); }

        public void LoadBob()
        {
            State = new Person { FirstName = "Bob", LastName = "Joe", Child = new Person { FirstName = "Kid1" } };
        }
        public void LoadBob2()
        {
            State = new Person { FirstName = "Bob", LastName = "Joe", Child = new Person { FirstName = "Kids2" } };
        }
        public void LoadFred()
        {
            State = new Person { FirstName = "Fred", LastName = "Mathews" };
        }

        public async void Mutate()
        {
            while (true)
            {
                await Task.Delay(100);

                State = BuildPersonWithPeople(State);
            }
        }

        private Person BuildPersonWithPeople(Person state)
        {
            return new Person
            {
                FirstName = "Mutating",
                LastName = "People",
                People = MutateList(state.People)
            };

        }

        private Random rnd = new Random();
        private ObservableCollection<Person> MutateList(ObservableCollection<Person> people)
        {
            var newPeople = people.ToList();
            var numChanges = rnd.Next(0, 1000) % 10;
            for (int i = 0; i < numChanges; i++)
            {
                if (newPeople.Count == 0)
                {
                    newPeople.Add(new Person { FirstName = $"Person: {DateTime.Now.ToLongTimeString()}" });
                    continue;
                }

                var next = rnd.Next(0, 1000) % 5;
                switch (next)
                {
                    case 0: // Add
                        newPeople.Add(new Person { FirstName = $"Person (Add): {DateTime.Now.ToLongTimeString()}" });
                        break;
                    case 1: // Remove
                        var removeIdx = rnd.Next(0, 1000) % newPeople.Count;
                        newPeople.RemoveAt(removeIdx);
                        break;
                    case 2: // Move
                        if (newPeople.Count < 2) continue;
                        var startIdx = rnd.Next(0, 1000) % newPeople.Count;
                        var movePerson = newPeople[startIdx];
                        newPeople.RemoveAt(startIdx);
                        var endIdx = rnd.Next(0, 1000) % newPeople.Count;
                        newPeople.Insert(endIdx, movePerson);
                        break;
                    case 3: // Change
                        var changeIdx = rnd.Next(0, 1000) % newPeople.Count;
                        var person = newPeople[changeIdx];
                        person.FirstName = "[changed] " + person.FirstName;
                        break;
                    case 4: // Insert
                        var insertIdx = rnd.Next(0, 1000) % newPeople.Count;
                        newPeople.Insert(insertIdx, new Person { FirstName = $"Person (Insert): {DateTime.Now.ToLongTimeString()}" });
                        break;
                }
            }

            return new ObservableCollection<Person>(newPeople);
        }
    }

    public class Person : NotifyBase, IRaisePropertyChanged
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Person Child { get; set; }

        public ObservableCollection<Person> People { get; set; } = new ObservableCollection<Person>();

        public void RaisePropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
        }
    }

    public interface IViewModelWithState<TState> : INotifyPropertyChanged
    {
        TState State { get; set; }
    }

    public interface IRaisePropertyChanged : INotifyPropertyChanged
    {
        void RaisePropertyChanged(string propertyName);
    }


    public class AppStateWrapper<TState> : StateWrapper<TState>
        where TState : class
    {
        public IViewModelWithState<TState> AppState { get; }

        public AppStateWrapper(IViewModelWithState<TState> state)
        {
            AppState = state;
            ChangeData();
            AppState.PropertyChanged += AppState_PropertyChanged;
        }

        private void AppState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppState.State))
            {
                ChangeData();
            }
        }

        private bool RunChangeData { get; set; }
        private int isRunningChangeData;
        private async void ChangeData()
        {
            var oldData = Data;
            var newData = AppState.State;
            RunChangeData = true;
            if (Interlocked.CompareExchange(ref isRunningChangeData, 0, 1) == 1) return;
            RunChangeData = false;
            try
            {
                if (oldData == null)
                {
                    if (newData != null && oldData != newData)
                    {
                        // There was no old data, so simply set the new data to be the current Data
                        // and raise PropertyChanged
                        Data = newData;
                        OnPropertyChanged(nameof(Data));
                    }

                    // Nothing more to do, so just return;
                    return;
                }

                if (newData == null)
                {
                    Data = newData;
                    OnPropertyChanged(nameof(Data));
                    return;
                }

                UpdateData(oldData, newData);
            }
            finally
            {
                Interlocked.Exchange(ref isRunningChangeData, 0);
            }
            await Task.Yield();
            if (RunChangeData)
            {
                ChangeData();
            }
        }

        private void UpdateData(object oldData, object newData)
        {
            var typeHelper = TypeHelper.RetrieveHelperForType(oldData.GetType());
            typeHelper.DeepUpdater(oldData, newData);
        }

        // private IDictionary<IRaisePropertyChanged, DataTracker> Trackers { get; } = new Dictionary<IRaisePropertyChanged, DataTracker>();

        //private void ReflectChanges(IRaisePropertyChanged oldData, IRaisePropertyChanged data)
        //{
        //    var type = oldData.GetType();
        //    if (type.IsValueType) return;

        //    var properties = type.GetProperties().Where(x => !x.PropertyType.IsValueType && x.PropertyType != typeof(string));
        //    foreach (var prop in properties)
        //    {
        //        var oldPropValue = prop.GetValue(oldData);
        //        var newPropValue = prop.GetValue(data);

        //        var oldHash = oldPropValue.Hash();
        //        var newHash = newPropValue.Hash();
        //        if (oldHash != newHash)
        //        {
        //            prop.SetValue(oldData, newPropValue);
        //            oldData.RaisePropertyChanged(prop.Name);
        //        }
        //        else
        //        {
        //            if (oldPropValue is IRaisePropertyChanged parent &&
        //                newPropValue is IRaisePropertyChanged newParent)
        //            {
        //                ReflectChanges(parent, newParent);
        //            }
        //        }
        //    }
        //}
    }

    //public class DataTracker
    //{
    //    public IRaisePropertyChanged Parent { get; set; }
    //    public object Child { get; set; }
    //}


    public class StateWrapper<TState> : NotifyBase
        where TState : class
    {
        public TState Data { get; set; }

    }


    public static class DataHelper
    {
        public static int Hash(this object entity)
        {
            if (entity == null) return 0;

            var type = entity.GetType();
            if (type.IsValueType) return entity.GetHashCode();

            var hashBuilder = TypeHelper.RetrieveHelperForType(type).HashBuilder;

            return hashBuilder(entity);
        }
    }

    public class TypeHelper
    {
        private Func<object, int> _hashBuilder;
        private Action<object, object> _deepUpdater;

        private static IDictionary<Type, TypeHelper> Helpers { get; } = new Dictionary<Type, TypeHelper>();
        public Type TypeToHelp { get; set; }
        public Func<object, int> HashBuilder { get => _hashBuilder ?? (_hashBuilder = CreateHashBuilder()); set => _hashBuilder = value; }


        public Action<object, object> DeepUpdater { get => _deepUpdater ?? (_deepUpdater = CreateDeepUpdater()); set => _deepUpdater = value; }

        private Func<object, int> CreateHashBuilder()
        {
            if (_hashBuilder == null)
            {
                SetupTypeHelper();
            }
            return _hashBuilder;
        }

        private Action<object, object> CreateDeepUpdater()
        {
            if (_deepUpdater == null)
            {
                SetupTypeHelper();
            }

            return _deepUpdater;
        }

        private void CollectionUpdater(object obj1, object obj2)
        {
            var type = obj1.GetType().GetGenericArguments();
            if (type == null) return;
            var updaterType = typeof(TypedCollectionUpdater<>).MakeGenericType(type);
            var updater = Activator.CreateInstance(updaterType, new object[] { obj1, obj2 }) as ICollectionUpdater;
            updater.Updater();
        }

        private interface ICollectionUpdater
        {
            Action Updater { get; }
        }
        private class TypedCollectionUpdater<T> : ICollectionUpdater
        {
            public ObservableCollection<T> List1 { get; }
            public ObservableCollection<T> List2 { get; }

            public Action Updater => Update;

            public TypedCollectionUpdater(object list1, object list2)
            {
                List1 = list1 as ObservableCollection<T>;
                List2 = list2 as ObservableCollection<T>;
            }


            private Action<object, object> _deepUpdater;
            private Action<object, object> DeepUpdater { get => _deepUpdater ?? (_deepUpdater = TypeHelper.RetrieveHelperForType(typeof(T)).DeepUpdater); }

            public void Update()
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

        private void SetupTypeHelper()
        {
            var hashBuilder = new Func<object, int>(obj => 0);
            var updater = new Action<object, object>((obj1, obj2) => { });

            if (TypeToHelp.GetInterfaces().Contains(typeof(INotifyCollectionChanged)))
            {
                _hashBuilder = hashBuilder;
                _deepUpdater = CollectionUpdater;
                return;
            }


            var valProperties = TypeToHelp.GetProperties().Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string));
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
            _hashBuilder = hashBuilder;

            var valUpdater = updater;



            var objectProperties = TypeToHelp.GetProperties().Where(x => !x.PropertyType.IsValueType && x.PropertyType != typeof(string));
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
                if (obj1 == null || obj2 == null) return;
                var obj1Hash = hashBuilder(obj1);
                var obj2Hash = hashBuilder(obj2);
                if (obj1Hash != obj2Hash)
                {
                    valUpdater(obj1, obj2);
                }
                objectUpdater(obj1, obj2);
            };
            _deepUpdater = updater;
        }

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
    }



    public static class HashHelper
    {
        //https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode/263416#263416
        public static int GetHashCode<T1, T2>(T1 arg1, T2 arg2)
        {
            unchecked
            {
                return arg1.GetHashCode().CombineHashCode(arg2.GetHashCode());
            }
        }

        public static int GetHashCode<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            unchecked
            {
                int hash = arg1.GetHashCode();
                hash = hash.CombineHashCode(arg2.GetHashCode());
                return hash.CombineHashCode(arg3.GetHashCode());
            }
        }

        public static int GetHashCode<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3,
            T4 arg4)
        {
            unchecked
            {
                int hash = arg1.GetHashCode();
                hash = hash.CombineHashCode(arg2.GetHashCode());
                hash = hash.CombineHashCode(arg3.GetHashCode());
                return hash.CombineHashCode(arg4.GetHashCode());
            }
        }

        public static int GetHashCode<T>(T[] list)
        {
            unchecked
            {
                int hash = 0;
                foreach (var item in list)
                {
                    hash = hash.CombineHashCode(item.GetHashCode());
                }
                return hash;
            }
        }

        public static int GetHashCode<T>(IEnumerable<T> list)
        {
            unchecked
            {
                int hash = 0;
                foreach (var item in list)
                {
                    hash = hash.CombineHashCode(item.GetHashCode());
                }
                return hash;
            }
        }

        /// <summary>
        /// Gets a hashcode for a collection for that the order of items 
        /// does not matter.
        /// So {1, 2, 3} and {3, 2, 1} will get same hash code.
        /// </summary>
        public static int GetHashCodeForOrderNoMatterCollection<T>(
            IEnumerable<T> list)
        {
            unchecked
            {
                int hash = 0;
                int count = 0;
                foreach (var item in list)
                {
                    hash += item.GetHashCode();
                    count++;
                }
                return hash.CombineHashCode(count.GetHashCode());
            }
        }

        /// <summary>
        /// Alternative way to get a hashcode is to use a fluent 
        /// interface like this:<br />
        /// return 0.CombineHashCode(field1).CombineHashCode(field2).
        ///     CombineHashCode(field3);
        /// </summary>
        public static int CombineHashCode<T>(this int hashCode, T arg)
        {
            unchecked
            {
                return 31 * hashCode + arg?.GetHashCode() ?? 0;
            }
        }
    }
}
