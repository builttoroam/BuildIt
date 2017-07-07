using System;

namespace BuildIt.States.Interfaces
{
    public interface IStateTrigger
    {
        event EventHandler IsActiveChanged;
        bool IsActive { get; }
    }
}