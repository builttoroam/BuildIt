using System;

namespace BuildIt.MvvmCross
{
    public class VisualStateAttribute : Attribute
    {
        public string VisualStateName { get; set; }

        public VisualStateAttribute(string visualStateName)
        {
            VisualStateName = visualStateName;
        }
    }
}