using BuildIt;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace General.Samples.Core
{

    public class MainViewModel : NotifyBase, IHasImmutableData<Person>
    {
        private Person _data;

        public Person Data { get => _data; set => SetProperty(ref _data, value); }

        public void LoadBob()
        {
            Data = new Person { FirstName = "Bob", LastName = "Joe", Child = new Person { FirstName = "Kid1" } };
        }
        public void LoadBob2()
        {
            Data = new Person { FirstName = "Bob", LastName = "Joe", Child = new Person { FirstName = "Kids2" } };
        }
        public void LoadFred()
        {
            Data = new Person { FirstName = "Fred", LastName = "Mathews" };
        }

        public async void Mutate()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(100);

                    Data = BuildPersonWithPeople(Data);
                }
            });
        }

        private Person BuildPersonWithPeople(Person state)
        {
            return new Person
            {
                FirstName = "Mutating",
                LastName = "People",
                People = MutateList(state.People)
            };

        }

        private Random rnd = new Random();
        private int deleteAllSupressCount;
        private ObservableCollection<Person> MutateList(ObservableCollection<Person> people)
        {
            var newPeople = people.ToList();
            var numChanges = rnd.Next(0, 1000) % 10;
            for (int i = 0; i < numChanges; i++)
            {
                if (newPeople.Count == 0)
                {
                    newPeople.Add(new Person { FirstName = $"Person: {DateTime.Now.ToLongTimeString()}" });
                    continue;
                }

                var next = rnd.Next(0, 1000) % 6;
                deleteAllSupressCount++;
                switch (next)
                {
                    case 0: // Add
                        newPeople.Add(new Person { FirstName = $"Person (Add): {DateTime.Now.ToLongTimeString()}" });
                        break;
                    case 1: // Remove
                        var removeIdx = rnd.Next(0, 1000) % newPeople.Count;
                        newPeople.RemoveAt(removeIdx);
                        break;
                    case 2: // Move
                        if (newPeople.Count < 2) continue;
                        var startIdx = rnd.Next(0, 1000) % newPeople.Count;
                        var movePerson = newPeople[startIdx];
                        newPeople.RemoveAt(startIdx);
                        var endIdx = rnd.Next(0, 1000) % newPeople.Count;
                        newPeople.Insert(endIdx, movePerson);
                        break;
                    case 3: // Change
                        var changeIdx = rnd.Next(0, 1000) % newPeople.Count;
                        var person = newPeople[changeIdx];
                        person.FirstName = "[changed] " + person.FirstName;
                        break;
                    case 4: // Insert
                        var insertIdx = rnd.Next(0, 1000) % newPeople.Count;
                        newPeople.Insert(insertIdx, new Person { FirstName = $"Person (Insert): {DateTime.Now.ToLongTimeString()}" });
                        break;
                    case 5: // Remove all
                        if (deleteAllSupressCount % 50 == 0)
                        {
                            newPeople.Clear();
                        }

                        break;
                }
            }

            return new ObservableCollection<Person>(newPeople);
        }
    }


    //public class DataTracker
    //{
    //    public IRaisePropertyChanged Parent { get; set; }
    //    public object Child { get; set; }
    //}
}
