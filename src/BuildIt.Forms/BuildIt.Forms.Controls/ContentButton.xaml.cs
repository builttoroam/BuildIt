using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using BuildIt.Forms.Core;
using System.Windows.Input;
using Xamarin.Forms.Internals;
using System.Runtime.CompilerServices;

namespace BuildIt.Forms.Controls
{
	public partial class ContentButton : ContentView
	{
        public event EventHandler Pressed;

        public static readonly BindableProperty CommandProperty = 
            BindableProperty.Create("Command", typeof(ICommand), typeof(ContentButton), null, 
                BindingMode.OneWay, null, new BindableProperty.BindingPropertyChangedDelegate(CommandChanged), null, null, null);

        private static void CommandParameterChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            ((ContentButton)bindable).CommandCanExecuteChanged(bindable, EventArgs.Empty);
        }

        public static readonly BindableProperty CommandParameterProperty = 
            BindableProperty.Create("CommandParameter", typeof(object), typeof(Button), null, 
                BindingMode.OneWay, null, new BindableProperty.BindingPropertyChangedDelegate(CommandParameterChanged), null, null, null);

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


        private bool IsEnabledCore
        {
            set
            {
                base.SetValueCore(VisualElement.IsEnabledProperty, value, SetValueFlags.None);
            }
        }


        public ICommand Command
        {
            get
            {
                return (ICommand)base.GetValue(ContentButton.CommandProperty);
            }
            set
            {
                base.SetValue(ContentButton.CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return base.GetValue(ContentButton.CommandParameterProperty);
            }
            set
            {
                base.SetValue(ContentButton.CommandParameterProperty, value);
            }
        }

        public ContentButton ()
		{
            InitializeComponent();

            this.PropertyChanged += ContentButton_PropertyChanged;
        }

        private void ContentButton_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsEnabled))
            {
                UpdateVisualState();
            }
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            UpdateVisualState();
        }

        private bool IsEntered { get; set; }
        private bool IsPressed { get; set; }

        private bool EnterRequired { get; set; }
        protected void OnTouchEffectAction(object sender, object args)
        {
            var touchArgs = args as TouchActionEventArgs;
            if (touchArgs == null) return;
            switch(touchArgs.Type)
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
                    if(IsEntered || !EnterRequired)
                    {
                        SendPressed();
                    }
                    break;
            }
            UpdateVisualState();
        }


        public void SendPressed()
        {
            Command?.Execute(CommandParameter) ;
            Pressed.SafeRaise(this);

        }


        private void UpdateVisualState()
        {
            if (IsEnabled)
            {
                if (IsPressed)
                {
                    VisualStateManager.GoToState(this, "Pressed");
                }
                else if (IsEntered)
                {
                    VisualStateManager.GoToState(this, "PointerOver");
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal");
                }
            }
            else
            {
                VisualStateManager.GoToState(this, "Disabled");

            }
        }

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
