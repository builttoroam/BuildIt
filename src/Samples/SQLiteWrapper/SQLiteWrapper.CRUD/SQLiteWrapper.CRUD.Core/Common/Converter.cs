using SQLiteWrapper.CRUD.Core.Models.Database;
using SQLiteWrapper.CRUD.Core.Models.Views;

namespace SQLiteWrapper.CRUD.Core.Common
{
    public class Converter
    {
        public static Person Convert(PersonEntity personEntity)
        {
            if (personEntity == null) return null;

            return new Person()
            {
                Id = personEntity.Id,
                Name = personEntity.Name,
                Surname = personEntity.Surname
            };
        }

        public static PersonEntity Convert(Person person)
        {
            if (person == null) return null;

            return new PersonEntity()
            {
                Id = person.Id,
                Name = person.Name,
                Surname = person.Surname
            };
        }
    }
}
