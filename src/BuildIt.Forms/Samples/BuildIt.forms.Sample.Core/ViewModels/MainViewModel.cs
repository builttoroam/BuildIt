using BuildIt.States;
using BuildIt.States.Interfaces;
using BuildIt.Forms.Parameters;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BuildIt.Forms.Sample.Core.ViewModels
{
    public enum SampleStates
    {
        Base,
        Show,
        Hide
    }

    public enum ItemStates
    {
        Base,
        ItemEnabled,
        ItemDisabled
    }

    public class ItemViewModel : NotifyBase, IHasStates
    {
        private bool _enabledVisibility;

        public ItemViewModel()
        {
            StateManager.Group<ItemStates>().DefineAllStates();
            EnabledVisibility = Random.Next(0, 100) > 50;

            //Debug.WriteLine($"Enabled - {EnabledVisibility}");
        }

        public IStateManager StateManager { get; } = new StateManager();

        public bool EnabledVisibility
        {
            get => _enabledVisibility;
            set
            {
                _enabledVisibility = value;
                OnPropertyChanged();
            }
        }

        private static Random Random { get; } = new Random();

        public async Task Init()
        {
            await StateManager.GoToState(EnabledVisibility ? ItemStates.ItemEnabled : ItemStates.ItemDisabled, true);
        }

        public async Task Toggle()
        {
            EnabledVisibility = !EnabledVisibility;

            var j = 1.0;
            for (int i = 0; i < 10000; i++)
            {
                j *= (i / 10000.00);
            }

            await StateManager.GoToState(EnabledVisibility ? ItemStates.ItemEnabled : ItemStates.ItemDisabled, true);
        }
    }

    public class MainViewModel : NotifyBase, IHasStates, IHasImmutableData<Person>
    {
        private ICommand pressedCommand;
        private ICommand cameraPreviewErrorCommand;
        private ICommand testCommand;
        private bool commandIsEnabled;
        private bool cameraFocusMode;

        private bool visible = true;

        private Person _data;

        private Random rnd = new Random();

        private int deleteAllSupressCount;

        public MainViewModel()
        {
            StateManager
                .Group<SampleStates>()
                .DefineAllStates();
        }

        public ICommand PressedCommand => pressedCommand ?? (pressedCommand = new Command(SwitchStates, () => CommandIsEnabled));

        public ICommand CameraPreviewErrorCommand => cameraPreviewErrorCommand ?? (cameraPreviewErrorCommand = new Command(ExecuteCameraPreviewErrorCommand));

        private void ExecuteCameraPreviewErrorCommand(object obj)
        {
            if (obj is CameraPreviewControlErrorParameters<CameraFocusMode> cameraFocusErrorParameters)
            {
                CameraFocusMode = cameraFocusErrorParameters.Data;
            }
        }

        public ICommand TestCommand => testCommand ?? (testCommand = new Command(() =>
        {
            
        }));

        public IStateManager StateManager { get; } = new StateManager();

        public ObservableCollection<ItemViewModel> Items { get; } = new ObservableCollection<ItemViewModel>();

        public ObservableCollection<ItemViewModel> MoreItems { get; } = new ObservableCollection<ItemViewModel>();

        public bool CommandIsEnabled
        {
            get => commandIsEnabled;
            set
            {
                commandIsEnabled = value;
                (PressedCommand as Command).ChangeCanExecute();

                //OnPropertyChanged(()=> PressedCommand);
            }
        }

        public CameraFocusMode CameraFocusMode
        {
            get => CameraFocusMode;
            set => SetProperty(ref cameraFocusMode, value);
        }

        public Person Data { get => _data; set => SetProperty(ref _data, value); }

        public void SwitchStates()
        {
            visible = !visible;
            StateManager.GoToState(visible ? SampleStates.Show : SampleStates.Hide);
        }

        public async Task Init()
        {
            // TODO To get the app to start quicker comment out the code that relates to Items and MoreItems
            //var items = new List<ItemViewModel>();
            //for (int i = 0; i < 2000; i++)
            //{
            //    var item = new ItemViewModel();
            //    await item.Init();
            //    items.Add(item);
            //}

            //Items.Fill(items);
            //MoreItems.Fill(items);

            // need to request runtime permissions for using the camera
            var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera, Permission.Storage);
            if (results.ContainsKey(Permission.Camera))
            {
                var status = results[Permission.Camera];
                if (status == PermissionStatus.Granted)
                {
                    // granted permissions to access the camera
                }
            }

            if (results.ContainsKey(Permission.Storage))
            {
                var status = results[Permission.Storage];
                if (status == PermissionStatus.Granted)
                {
                    // granted permissions to access storage
                }
            }
        }

        public void LoadBob()
        {
            Data = new Person { FirstName = "Bob", LastName = "Joe", Child = new Person { FirstName = "Kid1" } };
        }

        public void LoadBob2()
        {
            Data = new Person { FirstName = "Bob", LastName = "Joe", Child = new Person { FirstName = "Kids2" } };
        }

        public void LoadFred()
        {
            Data = new Person { FirstName = "Fred", LastName = "Mathews" };
        }

        public async void Mutate()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(100);

                    Data = BuildPersonWithPeople(Data);
                }
            });
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

        private ObservableCollection<Person> MutateList(ObservableCollection<Person> people)
        {
            var newPeople = people.ToList();
            var numChanges = rnd.Next(0, 1000) % 10;
            for (int i = 0; i < numChanges; i++)
            {
                if (newPeople.Count == 0)
                {
                    newPeople.Add(new Person { FirstName = $"Person: {DateTime.Now.ToString()}" });
                    continue;
                }

                var next = rnd.Next(0, 1000) % 6;
                deleteAllSupressCount++;
                switch (next)
                {
                    case 0: // Add
                        newPeople.Add(new Person { FirstName = $"Person (Add): {DateTime.Now.ToString()}" });
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
                        newPeople.Insert(insertIdx, new Person { FirstName = $"Person (Insert): {DateTime.Now.ToString()}" });
                        break;

                    case 5: // Remove all
                        if (deleteAllSupressCount % 50 == 0)
                        {
                            newPeople.Clear();
                        }

                        break;
                }
            }

            return new ObservableCollection<Person>(newPeople);
        }
    }

    public class Command : ICommand
    {
        private readonly Func<object, bool> _canExecute;

        private readonly Action<object> _execute;

        public Command(Action<object> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this._execute = execute;
        }

        public Command(Action execute) : this(delegate (object o)
        {
            execute();
        })
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
        }

        public Command(Action<object> execute, Func<object, bool> canExecute) : this(execute)
        {
            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }
            this._canExecute = canExecute;
        }

        public Command(Action execute, Func<bool> canExecute) : this(delegate (object o)
        {
            execute();
        }, (object o) => canExecute())
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }
        }

        public event EventHandler CanExecuteChanged;

        /// <param name="parameter">An <see cref="T:System.Object" /> used as parameter to determine if the Command can be executed.</param>
        /// <summary>Returns a <see cref="T:System.Boolean" /> indicating if the Command can be exectued with the given parameter.</summary>
        /// <returns>
        ///     <see langword="true" /> if the Command can be executed, <see langword="false" /> otherwise.</returns>
        /// <remarks>
        ///     <para>If no canExecute parameter was passed to the Command constructor, this method always returns <see langword="true" />.</para>
        ///     <para>If the Command was created with non-generic execute parameter, the parameter of this method is ignored.</para>
        /// </remarks>
        public bool CanExecute(object parameter)
        {
            return this._canExecute == null || this._canExecute(parameter);
        }

        /// <param name="parameter">An <see cref="T:System.Object" /> used as parameter for the execute Action.</param>
        /// <summary>Invokes the execute Action</summary>
        /// <remarks>
        ///     <para>If the Command was created with non-generic execute parameter, the parameter of this method is ignored.</para>
        /// </remarks>
        public void Execute(object parameter)
        {
            this._execute(parameter);
        }

        /// <summary>Send a <see cref="E:System.Windows.Input.ICommand.CanExecuteChanged" /></summary>
        /// <remarks />
        public void ChangeCanExecute()
        {
            EventHandler expr_06 = this.CanExecuteChanged;
            if (expr_06 == null)
            {
                return;
            }
            expr_06(this, EventArgs.Empty);
        }
    }

    public class Person : NotifyBase
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Person Child { get; set; }

        public ObservableCollection<Person> People { get; set; } = new ObservableCollection<Person>();
    }
}