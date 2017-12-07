using System.Collections.Generic;
using System.Threading.Tasks;
using BuildIt.Data.Sqlite.Database.Models.Results;
using BuildIt.Data.Sqlite.Repository;
using BuildIt.Data.Sqlite.Repository.Interfaces;

namespace BuildIt.Data.Sqlite.Database.Interfaces
{
    public interface IBaseDatabaseService
    {
        Task<List<TEntity>> Get<TEntity>() where TEntity : class, IBaseEntity, new();
        Task<List<TEntity>> GetWithChildren<TEntity>() where TEntity : class, IBaseEntity, new();
        Task<TEntity> Get<TEntity>(string entityId) where TEntity : class, IBaseEntity, new();

        Task<TEntity> GetWithChildren<TEntity>(string entityId) where TEntity : class, IBaseEntity, new();

        Task<ISaveDataResult> InsertOrUpdate<TEntity>(TEntity entity) where TEntity : class, IBaseEntity, new();
        Task<ISaveDataResult> InsertOrUpdateWithChildren<TEntity>(TEntity entity) where TEntity : class, IBaseEntity, new();

        Task<bool> UpdateWithChildren<TEntity>(TEntity entity) where TEntity : class, IBaseEntity, new();

        Task<IDataResult> Delete<TEntity>(string entityId) where TEntity : class, IBaseEntity, new();

        Task<List<T>> ExecuteQuery<T>(string sqlQuery) where T : class, new();
    }
}
