using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BuildIt.Forms.Controls
{
    /// <summary>
    /// Represents a button which can have content and uses a content template
    /// </summary>
    public partial class ContentButton
    {
        /// <summary>
        /// The command to be invoked when button pressed
        /// </summary>
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ContentButton), null, BindingMode.OneWay, null, CommandChanged, null, null, null);

        /// <summary>
        /// The command parameters
        /// </summary>
        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(Button), null, BindingMode.OneWay, null, CommandParameterChanged, null, null, null);

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentButton"/> class.
        /// </summary>
        public ContentButton()
        {
            InitializeComponent();

            PropertyChanged += ContentButton_PropertyChanged;
        }

        /// <summary>
        /// Pressed event
        /// </summary>
        public event EventHandler Pressed;

        /// <summary>
        /// Gets or sets the command to be invoked when button pressed
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);

            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the parameter to be sent to the command
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);

            set => SetValue(CommandParameterProperty, value);
        }

        private bool IsEnabledCore
        {
            set => SetValueCore(IsEnabledProperty, value);
        }

        private bool IsEntered { get; set; }

        private bool IsPressed { get; set; }

        private bool EnterRequired { get; set; }

        /// <summary>
        /// Invoked whenever the Parent of an element is set. Implement this method in order
        /// to add behavior when the element is added to a parent.
        /// </summary>
        protected override async void OnParentSet()
        {
            base.OnParentSet();

            await UpdateVisualState();
        }

        /// <summary>
        /// Sends the pressed event
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task SendPressed()
        {
            await Task.Yield();
            Command?.Execute(CommandParameter);
            Pressed.SafeRaise(this);
        }

        /// <summary>
        /// Raises the property changed event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == CommandProperty.PropertyName)
            {
                ICommand cmd = Command;
                if (cmd != null)
                {
                    cmd.CanExecuteChanged += new EventHandler(CommandCanExecuteChanged);
                }
            }
        }

        /// <summary>
        /// Raises the property changing event
        /// </summary>
        /// <param name="propertyName">The name of the property that's changing</param>
        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            if (propertyName == CommandProperty.PropertyName)
            {
                ICommand cmd = Command;
                if (cmd != null)
                {
                    cmd.CanExecuteChanged -= new EventHandler(CommandCanExecuteChanged);
                }
            }

            base.OnPropertyChanging(propertyName);
        }

        /// <summary>
        /// Invoked when a touch event happens and an event needs to be raised
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The touch event args</param>
        protected async void OnTouchEffectAction(object sender, object args)
        {
            try
            {
                var touchArgs = args as TouchActionEventArgs;
                if (touchArgs == null)
                {
                    return;
                }

                bool sendPressed = false;
                switch (touchArgs.Type)
                {
                    case TouchActionType.Entered:
                        EnterRequired = false;
                        IsEntered = true;
                        break;

                    case TouchActionType.Exited:
                        if (IsPressed)
                        {
                            EnterRequired = true;
                        }

                        IsEntered = false;
                        break;

                    case TouchActionType.Pressed:
                        IsPressed = true;
                        break;

                    case TouchActionType.Released:
                        IsPressed = false;
                        if (IsEntered || !EnterRequired)
                        {
                            sendPressed = true;
                        }

                        break;
                }

                await UpdateVisualState();

                // Send the pressed event after we've finished updating the visual state of the button
                if (sendPressed)
                {
                    await SendPressed();
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        private static void CommandParameterChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            ((ContentButton)bindable).CommandCanExecuteChanged(bindable, EventArgs.Empty);
        }

        private static void CommandChanged(BindableObject bo, object o, object n)
        {
            ((ContentButton)bo).OnCommandChanged();
        }

        private void OnCommandChanged()
        {
            if (Command != null)
            {
                Command.CanExecuteChanged += new EventHandler(CommandCanExecuteChanged);
                CommandCanExecuteChanged(this, EventArgs.Empty);
                return;
            }

            IsEnabledCore = true;
        }

        private async void ContentButton_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsEnabled))
            {
                await UpdateVisualState();
            }
        }

        private async Task UpdateVisualState()
        {
            if (IsEnabled)
            {
                if (IsPressed)
                {
                    await VisualStateManager.GoToState(this, "Pressed");

                    // Releasing the button after half a second. This should stop the button's visual state getting "stuck" in a ListView -RR
                    await Task.Delay(500);
                    if (VisualStateManager.GetVisualStateGroups(this)?.StateManager?.CurrentState("CommonStates") == "Pressed")
                    {
                        OnTouchEffectAction(this, new TouchActionEventArgs(0, TouchActionType.Released, Point.Zero, false));
                    }
                }
                else if (IsEntered)
                {
                    await VisualStateManager.GoToState(this, "PointerOver");
                }
                else
                {
                    await VisualStateManager.GoToState(this, "Normal");
                }
            }
            else
            {
                await VisualStateManager.GoToState(this, "Disabled");
            }
        }

        private void CommandCanExecuteChanged(object sender, EventArgs eventArgs)
        {
            ICommand cmd = Command;
            if (cmd != null)
            {
                IsEnabledCore = cmd.CanExecute(CommandParameter);
            }
        }
    }
}