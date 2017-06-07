using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using BuildIt.Forms.Core;

namespace BuildIt.Forms.Controls
{
	public partial class ContentButton : ContentView
	{
        public event EventHandler Pressed;
      

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
        private void OnTouchEffectAction(object sender, object args)
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
                        Pressed.SafeRaise(this);
                    }
                    break;
            }
            UpdateVisualState();
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

    }
}
