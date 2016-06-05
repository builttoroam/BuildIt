namespace BuildIt.General.UI
{
    public interface ICodeBehindViewModel<TViewModel> where TViewModel:class
    {
        ContextWrapper<TViewModel> Data { get; }
    }
}