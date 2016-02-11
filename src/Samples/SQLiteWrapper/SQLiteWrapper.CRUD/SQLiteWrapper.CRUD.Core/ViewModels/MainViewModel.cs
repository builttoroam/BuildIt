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
                var agency = new AgencyEntity() {Name = "Hollywood Foreign Press"};
                var newPerson = new PersonEntity()
                {
                    Name = Constants.Nick,
                    Surname = Constants.Cage,
                    Agency = agency
                };
                var createRes = await databaseService.InsertOrUpdateWithChildren(newPerson);
                if (createRes.Success)
                {
                    var personEntity = await databaseService.GetWithChildren<PersonEntity>(createRes.NewEntityId);
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
                var personsInDb = await databaseService.GetWithChildren<PersonEntity>();
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
                    if (person.Agency != null)
                    {
                        person.Agency.Name = "Bollywood Native Junket";
                    }
                    var updateRes = await databaseService.InsertOrUpdateWithChildren(Converter.Convert(person));
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
