using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BuiltToRoam.Data.SQLite.Database.Interfaces;
using BuiltToRoam.Data.SQLite.Database.Models.Results;
using BuiltToRoam.Data.SQLite.Repository;
using SQLite.Net;
using SQLite.Net.Attributes;

namespace BuiltToRoam.Data.SQLite.Database
{
    public abstract class BaseDatabaseService : IBaseDatabaseService
    {
        // ReSharper disable once InconsistentNaming
        protected abstract Task<SQLiteConnection> CreateSQLiteConnection();
        protected abstract void CreateDatabaseTables(SQLiteConnection dbConnection);

        public async Task<List<TEntity>> Get<TEntity>() where TEntity : BaseEntity<TEntity>
        {
            try
            {
                var database = await CreateSQLiteConnection();
                if (database == null) return null;

                using (var repo = new BaseRepository<TEntity>(database))
                {
                    return repo.Table.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Message);
            }

            return new List<TEntity>();
        }
        public async Task<TEntity> Get<TEntity>(string entityId) where TEntity : BaseEntity<TEntity>
        {
            if (string.IsNullOrEmpty(entityId)) return null;

            try
            {
                var database = await CreateSQLiteConnection();
                if (database == null) return null;

                using (var repo = new BaseRepository<TEntity>(database))
                {
                    return repo.Get(entityId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<BaseSaveEntityResult> InsertOrUpdate<TEntity>(TEntity entity) where TEntity : BaseEntity<TEntity>
        {
            var res = new BaseSaveEntityResult();

            if (entity == null) return res;

            try
            {
                var database = await CreateSQLiteConnection();
                if (database == null) return res;

                using (var repo = new BaseRepository<TEntity>(database))
                {
                    var insertRes = repo.InsertOrUpdate(entity);
                    if (insertRes)
                    {
                        res.NewEntityId = entity.Id;
                        res.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Message);
            }

            return res;
        }
        public async Task<BaseResult> Delete<TEntity>(string entityId) where TEntity : BaseEntity<TEntity>
        {
            var res = new BaseResult();

            if (string.IsNullOrEmpty(entityId)) return null;

            try
            {
                var database = await CreateSQLiteConnection();
                if (database == null) return res;

                using (var repo = new BaseRepository<TEntity>(database))
                {
                    var deleteRes = repo.Delete(entityId);
                    res.Success = deleteRes;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Message);
            }

            return res;
        }

        public async Task<List<T>> ExecuteQuery<T>(string sqlQuery) where T : class
        {
            var res = new List<T>();

            if (string.IsNullOrEmpty(sqlQuery)) return res;

            try
            {
                var dbConnection = await CreateSQLiteConnection();
                if (dbConnection == null) return res;

                return dbConnection.Query<T>(sqlQuery);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Message);
            }

            return res;
        }
    }
}
