namespace BuildIt.VisualStates
{
    public class DefaultValue<TElement, TPropertyValue> : IDefaultValue

    {
        public VisualStateValue<TElement, TPropertyValue> VisualStateValue { get; set; }

        public TPropertyValue Value { get; set; }


        public void RevertToDefault()
        {
            VisualStateValue.Setter(VisualStateValue.Element, Value);
        }
    }
}