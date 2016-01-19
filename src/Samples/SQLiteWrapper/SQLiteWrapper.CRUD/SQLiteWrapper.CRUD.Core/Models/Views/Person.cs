using Cirrious.MvvmCross.ViewModels;

namespace SQLiteWrapper.CRUD.Core.Models.Views
{
    public class Person : MvxNotifyPropertyChanged
    {
        public string Id { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        private string surname;
        public string Surname
        {
            get { return surname; }
            set
            {
                surname = value;
                RaisePropertyChanged(() => Surname);
            }
        }
    }
}
