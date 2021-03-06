﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using BuildIt.Data.Sqlite.Repository.Interfaces;
using SQLite;
using SQLiteNetExtensions.Extensions;

// ReSharper disable UnusedVariable

namespace BuildIt.Data.Sqlite.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, IBaseEntity, new()
    {
        private SQLiteConnection db;
        private bool isDisposed;

        public TableQuery<TEntity> Table => db.Table<TEntity>();

        private readonly AutoResetEvent concurrencyResetEvent = new AutoResetEvent(true);

        public BaseRepository(SQLiteConnection db)
        {
            this.db = db;
        }

        public TEntity Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return null;

                return db.Get<TEntity>(e => e.Id == id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public TEntity GetWithChildren(string id, bool recursive = false)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return null;

                return db.GetWithChildren<TEntity>(id, recursive);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public List<TEntity> GetWithChildren()
        {
            try
            {
                return db.GetAllWithChildren<TEntity>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Inserts entity to db
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="throwsOnError"></param>
        /// <returns>Returns true if entity inserterd or false if not</returns>
        public bool Insert(TEntity entity, bool throwsOnError = false)
        {
            try
            {
                return Execute(() =>
                {
                    var insertRes = db.Insert(entity, typeof(TEntity));
                    return insertRes > 0;
                }, throwsOnError);
            }
            catch (Exception ex)
            {
                ex.LogException();
                if (throwsOnError) throw;
                return false;
            }
        }

        /// <summary>
        /// Inserts entity to db or updates if entity already exists 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="throwsOnError"></param>
        /// <param name="withChildren"></param>
        /// <returns>Returns true if entity inserterd or false if not</returns>
        public bool InsertOrUpdate(TEntity entity, bool throwsOnError = false)
        {
            if (entity == null) return false;


            try
            {
                try
                {
                    return Execute(() =>
                    {
                        var result = db.InsertOrReplace(entity);
                        return result > 0;
                    }, throwsOnError);
                }
                finally
                {
                    concurrencyResetEvent.Set();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return false;
        }

        public bool InsertOrUpdateWithChildren(TEntity entity, bool throwsOnError = false, bool recursive = false)
        {
            if (entity == null) return false;


            try
            {
                try
                {
                    return Execute(() =>
                    {
                        db.InsertOrReplaceWithChildren(entity, recursive);
                        return true;
                    }, throwsOnError);
                }
                finally
                {
                    concurrencyResetEvent.Set();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return false;
        }

        public bool UpdateWithChildren(TEntity entity, bool throwsOnError = false)
        {
            if (entity == null) return false;


            try
            {
                try
                {
                    return Execute(() =>
                    {
                        db.UpdateWithChildren(entity);
                        return true;
                    }, throwsOnError);
                }
                finally
                {
                    concurrencyResetEvent.Set();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return false;
        }

        public bool Delete(string id, bool throwsOnError = false)
        {
            return Delete(Get(id));
        }
        public Dictionary<string, bool> Delete(IEnumerable<string> ids, bool throwsOnError = false)
        {
            var res = new Dictionary<string, bool>();
            if (ids == null) return res;

            foreach (var id in ids)
            {
                try
                {
                    if (string.IsNullOrEmpty(id)) continue;

                    var deleteRes = Delete(id);
                    res.Add(id, deleteRes);
                }
                // ReSharper disable once UnusedVariable
                catch (Exception ex)
                {
                    ex.LogException();
                    if (throwsOnError) throw;
                    res.Add(id, false);
                }
            }

            return res;
        }

        public bool Delete(TEntity entity, bool throwsOnError = false)
        {
            if (entity == null) return false;

            try
            {
                return Execute(() =>
                {
                    // MK returns number of rows deleted
                    var deleteRes = db.Delete<TEntity>(entity.Id);
                    return deleteRes > 0;
                }, throwsOnError);
            }
            catch (Exception ex)
            {
                ex.LogException();
                if (throwsOnError) throw;
                return false;
            }
        }

        public Dictionary<TEntity, bool> Delete(IEnumerable<TEntity> entities, bool throwsOnError = false)
        {
            var res = new Dictionary<TEntity, bool>();
            if (entities == null) return null;

            foreach (var entity in entities)
            {
                try
                {
                    var deleteRes = Delete(entity);
                    res.Add(entity, deleteRes);
                }
                // ReSharper disable once UnusedVariable
                catch (Exception ex)
                {
                    ex.LogException();
                    if (throwsOnError) throw;
                    res.Add(entity, false);
                }
            }

            return res;
        }

        public void DeleteAll(bool throwsOnError = false)
        {
            try
            {
                db.DeleteAll<TEntity>();
            }
            catch (Exception ex)
            {
                ex.LogException();
                if (throwsOnError) throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.       
                db = null;
            }

            // Free any unmanaged objects here.             
            isDisposed = true;
        }

        private bool Execute(Func<bool> action, bool throwsOnError = false)
        {
            try
            {
                if (concurrencyResetEvent.WaitOne())
                {
                    return action();
                }
            }
            // ReSharper disable once UnusedVariable
            catch (Exception ex)
            {
                ex.LogException();
                if (throwsOnError) throw;
                return false;
            }
            finally
            {
                concurrencyResetEvent.Set();
            }

            return false;
        }
    }
}
