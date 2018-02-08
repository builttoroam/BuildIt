using System;
using BuildIt.Data.Sqlite.Repository.Interfaces;
using SQLite;

namespace BuildIt.Data.Sqlite.Repository
{
    public abstract class BaseEntity : IBaseEntity 
    {
        [PrimaryKey]
        public string Id { get; set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }


    public abstract class BaseEntity<TEntity> : BaseEntity where TEntity : IBaseEntity
    {

        /// <summary>
        /// Override and write update logic for entity in it
        /// </summary>
        /// <param name="entityToBeUpdated"></param>
        /// <param name="entitySource"></param>
        /// <returns>Returns true when entity has changes and update has to be done. False when there are no changes</returns>
        public virtual bool UpdateFromEntity(TEntity entityToBeUpdated, TEntity entitySource)
        {
            return entityToBeUpdated.Id == entitySource.Id;
        }
    }
}
