using System;
using System.Collections.Generic;
using SQLite;

namespace BuildIt.Data.Sqlite.Repository.Interfaces
{
    public interface IBaseRepository<TEntity> : IDisposable where TEntity : IBaseEntity
    {
        TableQuery<TEntity> Table { get; }
        TEntity Get(string id);
        TEntity GetWithChildren(string id, bool recursive = false);
        List<TEntity> GetWithChildren();

        /// <summary>
        /// Inserts entity to db
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="throwsOnError"></param>
        /// <returns>Returns true if entity inserterd or false if not</returns>
        bool Insert(TEntity entity, bool throwsOnError = false);

        /// <summary>
        /// Inserts entity to db or updates if entity already exists 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="throwsOnError"></param>
        /// <param name="withChildren"></param>
        /// <returns>Returns true if entity inserterd or false if not</returns>
        bool InsertOrUpdate(TEntity entity, bool throwsOnError = false);

        bool InsertOrUpdateWithChildren(TEntity entity, bool throwsOnError = false, bool recursive = false);
        bool UpdateWithChildren(TEntity entity, bool throwsOnError = false);
        bool Delete(string id, bool throwsOnError = false);
        Dictionary<string, bool> Delete(IEnumerable<string> ids, bool throwsOnError = false);
        bool Delete(TEntity entity, bool throwsOnError = false);
        Dictionary<TEntity, bool> Delete(IEnumerable<TEntity> entities, bool throwsOnError = false);
        void DeleteAll(bool throwsOnError = false);
    }
}