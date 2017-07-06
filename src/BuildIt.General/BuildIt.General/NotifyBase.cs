using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace BuildIt
{
    /// <summary>
    /// Base implementation of INotifyPropertyChanged
    /// </summary>
    public class NotifyBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Method for invoking property changed
        /// </summary>
        /// <param name="propertyName">The name of the property to raise event for</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Method for invoking property changed
        /// </summary>
        /// <typeparam name="TValue">The property return type</typeparam>
        /// <param name="selector">An expression that points to the property to raise the changed event for</param>
        protected virtual void OnPropertyChanged<TValue>(Expression<Func<TValue>> selector)
        {
            var handler = PropertyChanged;
            var name = (selector.Body as MemberExpression)?.Member.Name;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}