using Android.App;
using Android.OS;
using Android.Widget;
using StateByState.Regions.Main;

namespace StateByState.Android
{
    [Activity(Label = "@string/SecondTitle")]
    public class SecondActivity : BaseActivity
    {
        public VisualStateWrapper<SecondStates> VisualStates { get; } = new VisualStateWrapper<SecondStates>();

        public Button MyButton1 { get; set; }
        public Button MyButton2 { get; set; }
        public Button MyButton3 { get; set; }

        public SecondViewModel CurrentViewModel => DataContext as SecondViewModel;

        public SecondActivity()
        {
            VisualStates.States[SecondStates.State1] = useTransitions =>
            {
                MyButton1.Text = "First";
            };

            VisualStates.States[SecondStates.State2] = useTransitions =>
            {
                MyButton2.Text = "Second";
            };

            VisualStates.States[SecondStates.State3] = useTransitions =>
            {
                MyButton3.Text = "Third";
            };
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Second);

            MyButton1 = FindViewById<Button>(Resource.Id.MyButton1);
            MyButton1.Click += (s, e) => CurrentViewModel.ToSecond();

            MyButton2 = FindViewById<Button>(Resource.Id.MyButton2);
            MyButton2.Click += (s, e) => CurrentViewModel.ToThird();
            MyButton3 = FindViewById<Button>(Resource.Id.MyButton3);
            MyButton3.Click += (s, e) => CurrentViewModel.ToFirst();
        }
    }
}