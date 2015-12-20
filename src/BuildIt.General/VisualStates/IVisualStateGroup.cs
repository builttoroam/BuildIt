namespace BuildIt.VisualStates
{
    public interface IVisualStateGroup
    {
        //IVisualState VisualState<TVisualState>(TVisualState state) where TVisualState : struct;
        void TransitionTo<TFindVisualState>(TFindVisualState state) where TFindVisualState : struct;

    }
}