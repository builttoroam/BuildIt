using System.Collections.Generic;
using System.Threading.Tasks;
using BuiltToRoam.Data.SQLite.Database.Models.Results;
using BuiltToRoam.Data.SQLite.Repository;

namespace BuiltToRoam.Data.SQLite.Database.Interfaces
{
    public interface IBaseDatabaseService
    {
        Task<List<TEntity>> Get<TEntity>() where TEntity : BaseEntity<TEntity>;
        Task<TEntity> Get<TEntity>(string entityId) where TEntity : BaseEntity<TEntity>;        

        Task<BaseSaveEntityResult> InsertOrUpdate<TEntity>(TEntity entity) where TEntity : BaseEntity<TEntity>;
        Task<BaseResult> Delete<TEntity>(string entityId) where TEntity : BaseEntity<TEntity>;

        Task<List<T>> ExecuteQuery<T>(string sqlQuery) where T : class;
    }
}
