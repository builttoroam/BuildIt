using System.Collections.Generic;
using System.Threading.Tasks;
using BuildIt.SQLiteWrapper.Database.Models.Results;
using BuildIt.SQLiteWrapper.Repository;

namespace BuildIt.SQLiteWrapper.Database.Interfaces
{
    public interface IBaseDatabaseService
    {
        Task<List<TEntity>> Get<TEntity>() where TEntity : BaseEntity<TEntity>;
        Task<List<TEntity>> GetWithChildren<TEntity>() where TEntity : BaseEntity<TEntity>;
        Task<TEntity> Get<TEntity>(string entityId) where TEntity : BaseEntity<TEntity>;

        Task<TEntity> GetWithChildren<TEntity>(string entityId) where TEntity : BaseEntity<TEntity>;

        Task<BaseSaveEntityResult> InsertOrUpdate<TEntity>(TEntity entity) where TEntity : BaseEntity<TEntity>;
        Task<BaseSaveEntityResult> InsertOrUpdateWithChildren<TEntity>(TEntity entity) where TEntity : BaseEntity<TEntity>;

        Task<BaseResult> Delete<TEntity>(string entityId) where TEntity : BaseEntity<TEntity>;

        Task<List<T>> ExecuteQuery<T>(string sqlQuery) where T : class;
    }
}
