using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildIt.Data.Sqlite.Repository;
using SQLiteNetExtensions.Attributes;

namespace SQLiteWrapper.CRUD.Core.Models.Database
{
    public class AgencyEntity : BaseEntity<AgencyEntity>
    {
        public string Name { get; set; }

        [OneToMany]
        public List<PersonEntity> Persons { get; set; } 
    }
}
