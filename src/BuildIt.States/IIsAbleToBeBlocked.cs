using System;

namespace BuildIt.States
{
    public interface IIsAbleToBeBlocked
    {
        event EventHandler IsBlockedChanged;

        bool IsBlocked { get; }
    }
}