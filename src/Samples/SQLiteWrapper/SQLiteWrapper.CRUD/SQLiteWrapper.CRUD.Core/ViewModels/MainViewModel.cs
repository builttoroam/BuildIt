using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;
using SQLiteWrapper.CRUD.Core.Common;
using SQLiteWrapper.CRUD.Core.Models.Database;
using SQLiteWrapper.CRUD.Core.Models.Views;
using SQLiteWrapper.CRUD.Core.Services.Interfaces;

namespace SQLiteWrapper.CRUD.Core.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        private readonly IDatabaseService databaseService;
        private readonly ILocalFileService localFileService;

        private ObservableCollection<Person> persons = new ObservableCollection<Person>();
        public ObservableCollection<Person> Persons
        {
            get { return persons; }
            set
            {
                persons = value;
                RaisePropertyChanged(() => Persons);
            }
        }

        public string DatabaseFilePath { get; set; }

        public MainViewModel(IDatabaseService databaseService, ILocalFileService localFileService)
        {
            this.databaseService = databaseService;
            this.localFileService = localFileService;
        }

        public void Init()
        {

        }

        public async override void Start()
        {
            base.Start();

            DatabaseFilePath = await localFileService.RetrieveNativePath(Constants.DatabaseName);
        }

        // CRUD
        public async Task Create()
        {
            try
            {
                var newPerson = new PersonEntity()
                {
                    Name = Constants.Nick,
                    Surname = Constants.Cage
                };
                var createRes = await databaseService.InsertOrUpdate(newPerson);
                if (createRes.Success)
                {
                    var personEntity = await databaseService.Get<PersonEntity>(createRes.NewEntityId);
                    var person = Converter.Convert(personEntity);
                    if (person != null)
                    {
                        Persons.Add(person);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        public async Task Read()
        {
            try
            {
                var personsInDb = await databaseService.Get<PersonEntity>();
                if (personsInDb == null) return;

                foreach (var p in personsInDb)
                {
                    // ReSharper disable once SimplifyLinqExpression
                    if (!Persons.Any(person => p.Id == person.Id))
                    {
                        Persons.Add(Converter.Convert(p));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        public async Task Update()
        {
            try
            {
                foreach (var person in Persons)
                {
                    if (person == null) continue;

                    person.Name = string.Equals(person.Name, Constants.Nick, StringComparison.CurrentCultureIgnoreCase) ? Constants.Nicholas : Constants.Nick;
                    var updateRes = await databaseService.InsertOrUpdate(Converter.Convert(person));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        public async Task Delete()
        {
            try
            {
                foreach (var personDictEntry in Persons.ToDictionary(p => p.Id))
                {
                    var delRes = await databaseService.Delete<PersonEntity>(personDictEntry.Key);
                    if (delRes.Success)
                    {
                        Persons.Remove(personDictEntry.Value);
                    }

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
