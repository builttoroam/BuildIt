using MvvmCross.Core.ViewModels;

namespace SQLiteWrapper.CRUD.Core.Models.Views
{
    public class Agency : MvxNotifyPropertyChanged
    {
        public string Id { get; set; }
        private string name;

        public string Name
        {
            get { return name; }

            set
            {

                if (name == value) return;

                name = value;
                RaisePropertyChanged(() => Name);
            }
        }
    }
}
