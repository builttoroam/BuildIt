using MvvmCross.Core.ViewModels;

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

        private Agency agency;

        public Agency Agency
        {
            get { return agency; }

            set
            {

                if (agency == value) return;

                agency = value;
                RaisePropertyChanged(() => Agency);
            }
        }
    }
}
