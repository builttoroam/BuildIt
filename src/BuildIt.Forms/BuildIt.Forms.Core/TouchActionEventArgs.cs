using System;
using Xamarin.Forms;

namespace BuildIt.Forms
{
    /// <summary>
    /// Touch event args - for when a touch event is raised.
    /// </summary>
    public class TouchActionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TouchActionEventArgs"/> class.
        /// </summary>
        /// <param name="id">The id of the touch action.</param>
        /// <param name="type">The type of touch input.</param>
        /// <param name="location">The location of the touch input.</param>
        /// <param name="isInContact">Whether touch is still in contact.</param>
        public TouchActionEventArgs(long id, TouchActionType type, Point location, bool isInContact)
        {
            Id = id;
            Type = type;
            Location = location;
            IsInContact = isInContact;
        }

        /// <summary>
        /// Gets the Id of the touch action.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the touch action type.
        /// </summary>
        public TouchActionType Type { get; }

        /// <summary>
        /// Gets the location of the touch input.
        /// </summary>
        public Point Location { get; }

        /// <summary>
        /// Gets a value indicating whether touch is in contact.
        /// </summary>
        public bool IsInContact { get; }
    }
}