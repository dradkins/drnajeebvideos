using DrNajeeb.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrNajeeb.Data.Helpers
{
    public class RepositoryProvider : IRepositoryProvider
    {
        #region Members

        private RepositoryFactories _repositoryFactories;

        protected Dictionary<Type, object> _Repositories { get; private set; }

        #endregion

        #region Constructor

        public RepositoryProvider(RepositoryFactories repositoryFactories)
        {
            _repositoryFactories = repositoryFactories;
            _Repositories = new Dictionary<Type, object>();
        }

        #endregion

        #region Class Member Function

        private T MakeRepository<T>(Func<DbContext, object> factory, DbContext _DbContext)
        {
            var f = factory ?? _repositoryFactories.GetRepositoryFactory<T>();
            if (f == null)
            {
                throw new NotImplementedException("No factory for repository type, " + typeof(T).FullName);
            }
            var repo = (T)f(_DbContext);
            _Repositories[typeof(T)] = repo;
            return repo;
        }

        #endregion

        #region Interface Implementation

        public DbContext _DbContext { get; set; }

        public IRepository<T> GetRepositoryForEntityType<T>() where T : class
        {
            return GetRepository<IRepository<T>>(
                _repositoryFactories.GetRepositoryFactoryForEntityType<T>());
        }

        public T GetRepository<T>(Func<System.Data.Entity.DbContext, object> factory = null) where T : class
        {
            object repoObject;
            _Repositories.TryGetValue(typeof(T), out repoObject);
            if (repoObject != null)
            {
                return (T)repoObject;
            }
            return MakeRepository<T>(factory, _DbContext);
        }

        public void SetRepository<T>(T repository)
        {
            _Repositories[typeof(T)] = repository;
        }

        #endregion
    }
}
