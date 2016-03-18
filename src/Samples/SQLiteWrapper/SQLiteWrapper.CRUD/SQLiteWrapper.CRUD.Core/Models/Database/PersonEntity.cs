using BuildIt.Data.Sqlite.Repository;
using SQLiteNetExtensions.Attributes;

namespace SQLiteWrapper.CRUD.Core.Models.Database
{
    public class PersonEntity : BaseEntity<PersonEntity>
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        [ForeignKey(typeof(AgencyEntity))]
        public string AgencyId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public AgencyEntity Agency { get; set; }
    }
}
