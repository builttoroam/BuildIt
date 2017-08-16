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
            BindableProperty.Create("Command", typeof(ICommand), typeof(ContentButton), null, BindingMode.OneWay, null, CommandChanged, null, null, null);

        /// <summary>
        /// The command parameters
        /// </summary>
        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create("CommandParameter", typeof(object), typeof(Button), null, BindingMode.OneWay, null, CommandParameterChanged, null, null, null);

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
            get => (ICommand)GetValue(ContentButton.CommandProperty);

            set => SetValue(ContentButton.CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the parameter to be sent to the command
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(ContentButton.CommandParameterProperty);

            set => SetValue(ContentButton.CommandParameterProperty, value);
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
        protected void SendPressed()
        {
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
            if (propertyName == ContentButton.CommandProperty.PropertyName)
            {
                ICommand cmd = this.Command;
                if (cmd != null)
                {
                    cmd.CanExecuteChanged += new EventHandler(this.CommandCanExecuteChanged);
                }
            }
        }

        /// <summary>
        /// Raises the property changing event
        /// </summary>
        /// <param name="propertyName">The name of the property that's changing</param>
        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            if (propertyName == ContentButton.CommandProperty.PropertyName)
            {
                ICommand cmd = this.Command;
                if (cmd != null)
                {
                    cmd.CanExecuteChanged -= new EventHandler(this.CommandCanExecuteChanged);
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
            var touchArgs = args as TouchActionEventArgs;
            if (touchArgs == null)
            {
                return;
            }

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
                        SendPressed();
                    }

                    break;
            }

            await UpdateVisualState();
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
            if (this.Command != null)
            {
                this.Command.CanExecuteChanged += new EventHandler(this.CommandCanExecuteChanged);
                this.CommandCanExecuteChanged(this, EventArgs.Empty);
                return;
            }

            this.IsEnabledCore = true;
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
            ICommand cmd = this.Command;
            if (cmd != null)
            {
                this.IsEnabledCore = cmd.CanExecute(this.CommandParameter);
            }
        }
    }
}
