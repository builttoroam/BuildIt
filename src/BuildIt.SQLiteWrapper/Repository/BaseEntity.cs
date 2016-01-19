using System;
using BuildIt.SQLiteWrapper.Repository.Interfaces;
using SQLite.Net.Attributes;

namespace BuildIt.SQLiteWrapper.Repository
{
    public abstract class BaseEntity<TEntity> : IBaseEntity where TEntity : IBaseEntity
    {
        [PrimaryKey]
        public string Id { get; set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
        }

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
