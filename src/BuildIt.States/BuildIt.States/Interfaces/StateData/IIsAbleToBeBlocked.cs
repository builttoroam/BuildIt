using System;

namespace BuildIt.States.Interfaces.StateData
{
    public interface IIsAbleToBeBlocked
    {
        event EventHandler IsBlockedChanged;

        bool IsBlocked { get; }
    }
}