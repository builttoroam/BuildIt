namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// State event args with cancel support.
    /// </summary>
    public interface IStateCancelEventArgs : IStateEventArgs, ICancellable
    {
    }
}