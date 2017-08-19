using System;
using BuildIt.States;
using BuildIt.States.Interfaces;
using Xamarin.Forms;

namespace StateByState.XForms.Views
{
    public partial class SecondPage : IHasStates
    {
        public IStateManager StateManager { get; }

        public SecondPage()
        {
            InitializeComponent();

            StateManager = new StateManager();
            StateManager
                .Group<SecondStates>()
                    .DefineState(SecondStates.State1)
                    .DefineState(SecondStates.State2)
                        .Target(textBlock)
                            .Change(x => x.TextColor, (x, c) => x.TextColor = c)
                            .ToValue(Color.FromHex("FFFF008B"))
                        .Target(textBlock)
                            .Change(x => x.FontSize, (x, c) => x.FontSize = c)
                            .ToValue(40)
                   .DefineState(SecondStates.State3)
                        .Target(textBlock)
                            .Change(x => x.TextColor, (x, c) => x.TextColor = c)
                            .ToValue(Color.FromHex("FFFFC500"))
                        .Target(textBlock)
                            .Change(x => x.FontSize, (x, c) => x.FontSize = c)
                            .ToValue(10)
                    .DefineState(SecondStates.State4)
                .Group<SecondStates2>()
                    .DefineState(SecondStates2.StateX)
                    .DefineState(SecondStates2.StateY)
                        .Target(textBlock2)
                            .Change(x => x.TextColor, (x, c) => x.TextColor = c)
                            .ToValue(Color.FromHex("FFFF008B"))
                        .Target(textBlock2)
                            .Change(x => x.FontSize, (x, c) => x.FontSize = c)
                            .ToValue(40)
                   .DefineState(SecondStates2.StateZ)
                        .Target(textBlock2)
                            .Change(x => x.TextColor, (x, c) => x.TextColor = c)
                            .ToValue(Color.FromHex("FFFFC500"))
                        .Target(textBlock2)
                            .Change(x => x.FontSize, (x, c) => x.FontSize = c)
                            .ToValue(10);




            //{
            //    VisualStateGroups =
            //    {
            //        {
            //            typeof (SecondStates), new VisualStateGroup<SecondStates>
            //            {
            //                VisualStates =
            //                {
            //                    {SecondStates.State1, new VisualState<SecondStates>(SecondStates.State1)
            //                        {
            //                            //Values =
            //                            //{
            //                            //    textBlock
            //                            //        .ChangeProperty(x => x.TextColor, (x, c) => x.TextColor = c)
            //                            //        .ToValue(Color.Blue)
            //                            //}
            //                        }
            //                    },
            //                    {SecondStates.State2, new VisualState<SecondStates>(SecondStates.State2)
            //                        {
            //                            Values =
            //                            {
            //                                textBlock
            //                                    .ChangeProperty(x => x.TextColor, (x, c) => x.TextColor = c)
            //                                    .ToValue(Color.FromHex("FFFF008B"))
            //                            }
            //                        }
            //                    },
            //                    {SecondStates.State3, new VisualState<SecondStates>(SecondStates.State3)
            //                        {
            //                            Values =
            //                            {
            //                                textBlock
            //                                    .ChangeProperty(x => x.TextColor, (x, c) => x.TextColor = c)
            //                                    .ToValue(Color.FromHex("#FFFFC500"))
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        },
            //        {typeof (SecondStates2), new VisualStateGroup<SecondStates2>{
            //                VisualStates =
            //                {
            //                    {SecondStates2.StateX, new VisualState<SecondStates2>(SecondStates2.StateX)
            //                        {
            //                            //Values =
            //                            //{
            //                            //    textBlock2
            //                            //        .ChangeProperty(x => x.TextColor, (x, c) => x.TextColor = c)
            //                            //        .ToValue(Color.Blue)
            //                            //}
            //                        }
            //                    },
            //                    {SecondStates2.StateY, new VisualState<SecondStates2>(SecondStates2.StateY)
            //                        {
            //                            Values =
            //                            {
            //                                textBlock2
            //                                    .ChangeProperty(x => x.TextColor, (x, c) => x.TextColor = c)
            //                                    .ToValue(Color.FromHex("FFFF008B"))
            //                            }
            //                        }
            //                    },
            //                    {SecondStates2.StateZ, new VisualState<SecondStates2>(SecondStates2.StateZ)
            //                        {
            //                            Values =
            //                            {
            //                                textBlock2
            //                                    .ChangeProperty(x => x.TextColor, (x, c) => x.TextColor = c)
            //                                    .ToValue(Color.FromHex("#FFFFC500"))
            //                            }
            //                        }
            //                    }
            //                }
            //            }}

            //    }
            //};
        }

        private SecondViewModel CurrentViewModel => BindingContext as SecondViewModel;

        private void ToFirst(object sender, EventArgs e)
        {
            CurrentViewModel.ToFirst();
        }

        private void ToSecond(object sender, EventArgs e)
        {
            CurrentViewModel.ToSecond();
        }

        private void ToThird(object sender, EventArgs e)
        {
            CurrentViewModel.ToThird();
        }

        private void Done(object sender, EventArgs e)
        {
            CurrentViewModel.Done();
        }

        private void XtoZ(object sender, EventArgs e)
        {
            CurrentViewModel.XtoZ();
        }

        private void YtoZ(object sender, EventArgs e)
        {
            CurrentViewModel.YtoZ();
        }

        private void ZtoY(object sender, EventArgs e)
        {
            CurrentViewModel.ZtoY();
        }

        private void YtoX(object sender, EventArgs e)
        {
            CurrentViewModel.YtoX();
        }
    }

}
