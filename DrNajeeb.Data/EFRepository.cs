using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrNajeeb.Contract;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;

namespace DrNajeeb.Data
{
    public class EFRepository<T> : IRepository<T> where T : class
    {
        #region Class Members

        protected DbContext _DbContext { get; set; }
        public DbSet<T> _DbSet { get; set; }

        #endregion

        #region Constructors

        public EFRepository(DbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException("dbContext");
            }
            this._DbContext = dbContext;
            _DbSet = this._DbContext.Set<T>();
        }

        #endregion

        #region IRepository Implementation

        public IQueryable<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return _DbSet.Where(predicate);
            }
            return _DbSet;
        }

        public async Task<T> GetAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return await _DbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<T> GetByIdAsync(int Id)
        {
            return await _DbSet.FindAsync(Id);
        }

        public async Task<T> GetByIdAsync(string Id)
        {
            return await _DbSet.FindAsync(Id);
        }

        public void Add(T entity)
        {
            DbEntityEntry dbEntityEntry = _DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                _DbSet.Add(entity);
            }
        }

        public void Update(T entity)
        {
            DbEntityEntry dbEntityEntry = _DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                _DbSet.Attach(entity);
            }
            dbEntityEntry.State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = _DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                _DbSet.Attach(entity);
                _DbSet.Remove(entity);
            }
        }

        public async Task DeleteAsync(int Id)
        {
            var entity = await GetByIdAsync(Id);
            if (entity == null) return; // not found; assume already deleted.
            Delete(entity);
        }

        public async Task DeleteAsync(string Id)
        {
            var entity = await GetByIdAsync(Id);
            if (entity == null) return; // not found; assume already deleted.
            Delete(entity);
        }

        public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
                return await _DbSet.CountAsync(predicate);
            else
                return await _DbSet.CountAsync();
        }

        public int Count(System.Linq.Expressions.Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
                return _DbSet.Count(predicate);
            else
                return _DbSet.Count();
        }

        #endregion
    }
}
