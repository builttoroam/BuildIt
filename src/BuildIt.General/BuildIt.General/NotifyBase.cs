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

        /// <summary>
        /// Sets a backing field and raises PropertyChanged if required
        /// </summary>
        /// <typeparam name="T">The type of the field to update</typeparam>
        /// <param name="storage">The reference to the field</param>
        /// <param name="value">The value to update the field with</param>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>Indicates if the property has changed</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            // ReSharper disable once ExplicitCallerInfoArgument - don't want "SetProperty" to be passed in
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}