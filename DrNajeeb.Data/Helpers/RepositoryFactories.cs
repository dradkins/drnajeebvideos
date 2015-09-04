using DrNajeeb.Contract;
using DrNajeeb.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrNajeeb.Data.Helpers
{
    public class RepositoryFactories
    {
        #region Class Members

        private readonly IDictionary<Type, Func<DbContext, object>> _repositoryFactories;

        #endregion

        #region Constructor

        public RepositoryFactories()
        {
            _repositoryFactories = GetFactories();
        }

        public RepositoryFactories(IDictionary<Type, Func<DbContext, object>> factories)
        {
            _repositoryFactories = factories;
        }

        #endregion

        #region Class Member Functions

        private IDictionary<Type, Func<DbContext, object>> GetFactories()
        {
            return new Dictionary<Type, Func<DbContext, object>>
            {
                //{typeof(IOrderRepository), x=>new OrderRepository(x)},
            };
        }

        public Func<DbContext, object> GetRepositoryFactory<T>()
        {
            Func<DbContext, object> factory;
            _repositoryFactories.TryGetValue(typeof(T), out factory);
            return factory;
        }

        public Func<DbContext, object> GetRepositoryFactoryForEntityType<T>() where T : class
        {
            return GetRepositoryFactory<T>() ?? DefaultEntityRepositoryFactory<T>();
        }

        private Func<DbContext, object> DefaultEntityRepositoryFactory<T>() where T : class
        {
            return x => new EFRepository<T>(x);
        }

        #endregion
    }
}
