using SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace SharedKernel.Interfaces
{
    public partial interface IRepository
    {
        IEnumerable<T> GetAll<T>() where T : class;
        T GetById<T>(int id) where T : class;
        List<T> ListWithWhere2<T>(Expression<Func<T, bool>> expression) where T : class;
        List<T> ListWithWhere<T>(Expression<Func<T, bool>> expression) where T : BaseEntity;
        Task<List<T>> ListAsyncWithWhere<T>(Expression<Func<T, bool>> expression) where T : BaseEntity;
        Task<List<T>> ListAsyncWithWhereIgnoreFilter<T>(Expression<Func<T, bool>> expression) where T : BaseEntity;
        Task<T> FirstAsyncWithWhere<T>(Expression<Func<T, bool>> expression) where T : BaseEntity;
        Task<List<T>> QueryAsync<T>(string query, Dictionary<string, object> param = null, bool isStoredProcedure = false);
        Task ExecuteQueryAsync(string query, Dictionary<string, object> param = null, bool isStoredProcedure = false);
        IEnumerable<T> ExecuteProcedure<T>(string spName, object param);
        Task<IEnumerable<T>> ExecuteProcedureAsync<T>(string spName, object param);
        Task<List<object>> ExecuteMultipleProcedure(string spName, object param, params Func<GridReader, object>[] readerFuncts);
        IQueryable<T> QueryableWithWhereAsync<T>(Expression<Func<T, bool>> expression) where T : BaseEntity;
        IQueryable<T> GetQueryable<T>() where T : BaseEntity;
        IQueryable<T> GetQueryable<T>(int maxRow) where T : BaseEntity;
        Task<IQueryable<T>> GetQueryableAsync<T>(int maxRow) where T : BaseEntity;
        Task<T> FindByIdAsync<T>(int id) where T : BaseEntity;
        Task<T> FindByIdAsyncIgnoreFilter<T>(int id) where T : BaseEntity;
        Task<(bool added, string message)> AddAsync<T>(T entity, bool isCommit = false) where T : BaseEntity;
        Task<(bool added, string message)> AddAsyncWithoutBaseEntity<T>(T entity, bool isCommit = false) where T : class;
        Task<(bool added, string message)> AddRangeAsync<T>(IEnumerable<T> entities, bool isCommit = false) where T : BaseEntity;
        Task<(bool added, string message)> AddRangeAsyncWithoutBaseEntity<T>(IEnumerable<T> entities, bool isCommit = false) where T : class;
    }
}
