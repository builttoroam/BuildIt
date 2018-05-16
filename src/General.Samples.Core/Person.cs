using System.Collections.ObjectModel;
using BuildIt;

namespace General.Samples.Core
{
    public class Person : NotifyBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Person Child { get; set; }

        public ObservableCollection<Person> People { get; set; } = new ObservableCollection<Person>();
    }
}