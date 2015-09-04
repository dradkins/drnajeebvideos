using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DrNajeeb.Contract
{
    public interface IRepository<T> where T:class
    {
        /// <summary>
        /// Get all values in repository
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// Get single or first value matching the given expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get single value by id
        /// </summary>
        /// <param name="Id">id of value to be retrieved</param>
        /// <returns></returns>
        Task<T> GetByIdAsync(int Id);

        /// <summary>
        /// Get single value by id
        /// </summary>
        /// <param name="Id">id of value to be retrieved</param>
        /// <returns></returns>
        Task<T> GetByIdAsync(string Id);

        /// <summary>
        /// Add value in repository
        /// </summary>
        /// <param name="entity">value to add</param>
        void Add(T entity);

        /// <summary>
        /// Update value in repository
        /// </summary>
        /// <param name="entity">value to be update</param>
        void Update(T entity);

        /// <summary>
        /// Delete value in repository
        /// </summary>
        /// <param name="entity">value to be delete</param>
        void Delete(T entity);

        /// <summary>
        /// Delete value in repository by id
        /// </summary>
        /// <param name="Id">id of value to be deleted</param>
        Task DeleteAsync(int Id);

        /// <summary>
        /// Delete value in repository by id
        /// </summary>
        /// <param name="Id">id of value to be deleted</param>
        Task DeleteAsync(string Id);

        /// <summary>
        /// Total number of values in repository
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// Total number of values in repository
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> predicate = null);
    }
}
