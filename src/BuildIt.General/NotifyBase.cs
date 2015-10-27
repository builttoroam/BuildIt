using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace BuildIt
{
    public class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void OnPropertyChanged<TValue>(Expression<Func<TValue>> selector)
        {
            var handler = PropertyChanged;
            var name = (selector.Body as MemberExpression)?.Member.Name;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}