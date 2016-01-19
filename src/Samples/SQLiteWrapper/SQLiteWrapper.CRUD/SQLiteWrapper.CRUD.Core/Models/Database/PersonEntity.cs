using BuiltToRoam.Data.SQLite.Repository;

namespace SQLiteWrapper.CRUD.Core.Models.Database
{
    public class PersonEntity : BaseEntity<PersonEntity>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
