namespace BuildIt.UI
{
    public interface IEventMap
    {
        void Wire(object viewModel);

        void Unwire(object viewModel);
    }
}