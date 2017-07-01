using System;

namespace BuildIt.States.Interfaces
{
    public interface IIsAbleToBeBlocked
    {
        event EventHandler IsBlockedChanged;

        bool IsBlocked { get; }
    }
}