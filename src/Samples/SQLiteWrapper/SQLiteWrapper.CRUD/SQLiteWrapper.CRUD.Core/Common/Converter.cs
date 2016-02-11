using SQLiteWrapper.CRUD.Core.Models.Database;
using SQLiteWrapper.CRUD.Core.Models.Views;

namespace SQLiteWrapper.CRUD.Core.Common
{
    public class Converter
    {
        public static Person Convert(PersonEntity personEntity)
        {
            if (personEntity == null) return null;

            var person = new Person()
            {
                Id = personEntity.Id,
                Name = personEntity.Name,
                Surname = personEntity.Surname,

            };
            if (personEntity.Agency != null)
            {
                person.Agency = new Agency() {Id = personEntity.AgencyId, Name = personEntity.Agency.Name};
            }
            return person;
        }

        public static PersonEntity Convert(Person person)
        {
            if (person == null) return null;

            var personEntity = new PersonEntity()
            {
                Id = person.Id,
                Name = person.Name,
                Surname = person.Surname,
                
            };
            if (person.Agency != null)
            {
                personEntity.Agency = new AgencyEntity() {Id = person.Agency.Id, Name = person.Agency.Name};
            }
            return personEntity;
        }
    }
}
