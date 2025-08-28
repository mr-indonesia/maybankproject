using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SharedKernel.Entities;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace DataAccess.EFCore.Repositories
{
    public partial class EFRepository : IRepository
    {
        protected readonly ApplicationContext context;

        public EFRepository() { }

        public EFRepository(ApplicationContext _context)
        {
            context = _context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            return context.Set<T>().ToList();
        }

        public T GetById<T>(int id) where T : class
        {
            return context.Set<T>().Find(id);
        }

        public List<T> ListWithWhere2<T>(Expression<Func<T, bool>> expression) where T : class
        {
            try
            {
                var query = context.Set<T>() as IQueryable<T>;
                query = query.Where(expression);

                return query.ToList();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public List<T> ListWithWhere<T>(Expression<Func<T, bool>> expression) where T : BaseEntity
        {
            try
            {
                var query = context.Set<T>() as IQueryable<T>;
                query = query.Where(expression);

                return query.ToList();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<T>> ListAsyncWithWhere<T>(Expression<Func<T, bool>> expression) where T : BaseEntity
        {
            try
            {
                var Query = context.Set<T>() as IQueryable<T>;
                Query = Query.Where(expression);

                return await Query.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<T>> ListAsyncWithWhereIgnoreFilter<T>(Expression<Func<T, bool>> expression) where T : BaseEntity
        {
            try
            {
                var query = context.Set<T>().IgnoreQueryFilters().AsNoTracking() as IQueryable<T>;
                query = query.Where(expression);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<T> FirstAsyncWithWhere<T>(Expression<Func<T, bool>> expression) where T : BaseEntity
        {
            try
            {
                var query = context.Set<T>() as IQueryable<T>;
                query = query.Where(expression);

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<T>> QueryAsync<T>(string query, Dictionary<string, object> param = null, bool isStoredProcedure = false) {
            if(isStoredProcedure)
            {
                return (await context.Database.GetDbConnection().QueryAsync<T>(query, param, commandTimeout: 120, commandType: CommandType.StoredProcedure)).ToList();
            }
            return (await context.Database.GetDbConnection().QueryAsync<T>(query, param, commandTimeout: 120)).ToList();
        }

        public async Task ExecuteQueryAsync(string query, Dictionary<string, object> param = null, bool isStoredProcedure = false)
        {
            if (isStoredProcedure) {
                await context.Database.GetDbConnection().ExecuteAsync(query, param, commandTimeout: 120, commandType: CommandType.StoredProcedure);
            }
            await context.Database.GetDbConnection().ExecuteAsync(query, param, commandTimeout: 120);
        }

        public IEnumerable<T> ExecuteProcedure<T>(string spName, object param)
        {
            return context.Database.GetDbConnection().Query<T>(spName, param, commandTimeout: 120, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<T>> ExecuteProcedureAsync<T>(string spName, object param)
        {
            return await context.Database.GetDbConnection().QueryAsync<T>(spName, param, commandTimeout: 120, commandType: CommandType.StoredProcedure);
        }
        public async Task<List<object>> ExecuteMultipleProcedure(string spName, object param, params Func<GridReader, object>[] readerFuncts)
        {
            var result = new List<object>();

            var gridReader = await context.Database.GetDbConnection().QueryMultipleAsync(spName, param, commandTimeout: 120, commandType: CommandType.StoredProcedure);

            foreach (var reader in readerFuncts)
            {
                var obj = reader(gridReader);
                result.Add(obj);
            }

            return result;
        }

        public IQueryable<T> QueryableWithWhereAsync<T>(Expression<Func<T, bool>> expression) where T : BaseEntity {
            try
            {
                var query = context.Set<T>() as IQueryable<T>;
                query = query.Where(expression);

                return query;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public IQueryable<T> GetQueryable<T>() where T : BaseEntity
        {
            try
            {
                var query = context.Set<T>() as IQueryable<T>;

                return query;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public IQueryable<T> GetQueryable<T>(int maxRow) where T : BaseEntity
        {
            try
            {
                var query = context.Set<T>() as IQueryable<T>;

                if(maxRow > 0)
                {
                    query = query.Take(maxRow).OrderBy(o => o.Id);
                }

                return query;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<IQueryable<T>> GetQueryableAsync<T>(int maxRow) where T : BaseEntity
        {
            try
            {
                var query = await context.Set<T>().AsQueryable<T>().ToListAsync<T>();
                if (maxRow > 0)
                {
                    query = query.Take(maxRow).OrderBy(o => o.Id).ToList();
                }


                return query.AsQueryable<T>();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<T> FindByIdAsync<T>(int id) where T : BaseEntity
        {
            try
            {
                var query = context.Set<T>() as IQueryable<T>;
                query = query.Where(e => e.Id == id);

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<T> FindByIdAsyncIgnoreFilter<T>(int id) where T : BaseEntity
        {
            try
            {
                var query = context.Set<T>().IgnoreQueryFilters().AsNoTracking() as IQueryable<T>;
                query = query.Where(e => e.Id == id);

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool added, string message)> AddAsync<T>(T entity, bool isCommit = false) where T : BaseEntity
        {
            try
            {
                if (isCommit)
                {
                    await context.Set<T>().AddAsync(entity);
                    await context.SaveChangesAsync();

                    return (true, "success");
                }
                else
                {
                    await context.Set<T>().AddAsync(entity);
                    return (true, "data successfully added without commit");
                }
            }
            catch (Exception ex)
            {

                if(ex.InnerException != null)
                {
                    return (false, "Trouble happened \n" + ex.Message + "\n" + ex.InnerException.Message);
                }
                else
                {
                    return (false, "Trouble happened \n" + ex.Message);
                }
            }
        }

        public async Task<(bool added, string message)> AddAsyncWithoutBaseEntity<T>(T entity, bool isCommit = false) where T : class
        {
            try
            {
                if (isCommit)
                {
                    await context.Set<T>().AddAsync(entity);
                    await context.SaveChangesAsync();

                    return (true, "success");
                }
                else
                {
                    await context.Set<T>().AddAsync(entity);
                    return (true, "data successfully added without commit");
                }
            }
            catch (Exception ex)
            {

                if (ex.InnerException != null)
                {
                    return (false, "Trouble happened \n" + ex.Message + "\n" + ex.InnerException.Message);
                }
                else
                {
                    return (false, "Trouble happened \n" + ex.Message);
                }
            }
        }

        public async Task<(bool added, string message)> AddRangeAsync<T>(IEnumerable<T> entities, bool isCommit = false) where T : BaseEntity {
            try
            {
                if (isCommit)
                {
                    await context.Set<T>().AddRangeAsync(entities);
                    await context.SaveChangesAsync();

                    return (true, "success");
                }
                else
                {
                    await context.Set<T>().AddRangeAsync(entities);
                    return (true, "data successfully added without commit");
                }
            }
            catch (Exception ex)
            {

                if (ex.InnerException != null)
                {
                    return (false, "Trouble happened \n" + ex.Message + "\n" + ex.InnerException.Message);
                }
                else
                {
                    return (false, "Trouble happened \n" + ex.Message);
                }
            }
        }

        public async Task<(bool added, string message)> AddRangeAsyncWithoutBaseEntity<T>(IEnumerable<T> entities, bool isCommit = false) where T : class {
            try
            {
                if (isCommit)
                {
                    await context.Set<T>().AddRangeAsync(entities);
                    await context.SaveChangesAsync();

                    return (true, "success");
                }
                else
                {
                    await context.Set<T>().AddRangeAsync(entities);
                    return (true, "data successfully added without commit");
                }
            }
            catch (Exception ex)
            {

                if (ex.InnerException != null)
                {
                    return (false, "Trouble happened \n" + ex.Message + "\n" + ex.InnerException.Message);
                }
                else
                {
                    return (false, "Trouble happened \n" + ex.Message);
                }
            }
        }
    }
}
