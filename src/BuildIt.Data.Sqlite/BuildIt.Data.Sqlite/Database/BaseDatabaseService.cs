﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BuildIt.Data.Sqlite.Database.Interfaces;
using BuildIt.Data.Sqlite.Database.Models.Results;
using BuildIt.Data.Sqlite.Repository;
using BuildIt.Data.Sqlite.Repository.Interfaces;
using SQLite;

namespace BuildIt.Data.Sqlite.Database
{
    public abstract class BaseDatabaseService : IBaseDatabaseService
    {
        // ReSharper disable once InconsistentNaming
        protected abstract Task<SQLiteConnection> CreateSQLiteConnection();
        protected abstract void CreateDatabaseTables(SQLiteConnection dbConnection);

        public async Task<List<TEntity>> Get<TEntity>() where TEntity : class, IBaseEntity, new()
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

        public async Task<List<TEntity>> GetWithChildren<TEntity>() where TEntity : class, IBaseEntity, new()
        {
            try
            {
                var database = await CreateSQLiteConnection();
                if (database == null) return null;

                using (var repo = new BaseRepository<TEntity>(database))
                {
                    return repo.GetWithChildren();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Message);
            }

            return new List<TEntity>();
        }

        public async Task<TEntity> Get<TEntity>(string entityId) where TEntity : class, IBaseEntity, new()
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

        public async Task<TEntity> GetWithChildren<TEntity>(string entityId) where TEntity : class, IBaseEntity, new()
        {
            if (string.IsNullOrEmpty(entityId)) return null;

            try
            {
                var database = await CreateSQLiteConnection();
                if (database == null) return null;

                using (var repo = new BaseRepository<TEntity>(database))
                {
                    return repo.GetWithChildren(entityId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<ISaveDataResult> InsertOrUpdate<TEntity>(TEntity entity) where TEntity : class, IBaseEntity, new()
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

        public async Task<ISaveDataResult> InsertOrUpdateWithChildren<TEntity>(TEntity entity) where TEntity : class, IBaseEntity, new()
        {
            var res = new BaseSaveEntityResult();

            if (entity == null) return res;

            try
            {
                var database = await CreateSQLiteConnection();
                if (database == null) return res;

                using (var repo = new BaseRepository<TEntity>(database))
                {
                    var insertRes = repo.InsertOrUpdateWithChildren(entity);
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

        /// <summary>
        /// Method is provided as it is recommended to avoid performance impact of InsertOrReaplceWithChildren that will delete and re-insert objects
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateWithChildren<TEntity>(TEntity entity) where TEntity : class, IBaseEntity, new()
        {
            try
            {
                var database = await CreateSQLiteConnection();
                using (var repo = new BaseRepository<TEntity>(database))
                {
                    var res = repo.UpdateWithChildren(entity);
                    return res;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Message);
            }

            return false;
        }

        public async Task<IDataResult> Delete<TEntity>(string entityId) where TEntity : class, IBaseEntity, new()
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

        public async Task<List<T>> ExecuteQuery<T>(string sqlQuery) where T : class, new()
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
