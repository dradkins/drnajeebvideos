using DrNajeeb.Contract;
using DrNajeeb.EF;
using DrNajeeb.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrNajeeb.Data
{
    public class Uow : IUow
    {
        #region Class Members

        protected IRepositoryProvider _RepositoryProvider { get; set; }

        private Entities _DbContext { get; set; }


        #endregion

        #region Constructor

        public Uow(IRepositoryProvider repositoryProvider)
        {
            CreateDbContext();

            repositoryProvider._DbContext = _DbContext;
            _RepositoryProvider = repositoryProvider;
        }

        #endregion

        #region Repositories

        #region Generic repositories

        public IRepository<Subscription> _Subscription { get { return GetStandardRepo<Subscription>(); } }

        public IRepository<Category> _Categories { get { return GetStandardRepo<Category>(); } }
        public IRepository<Video> _Videos { get { return GetStandardRepo<Video>(); } }
        public IRepository<Country> _Country { get { return GetStandardRepo<Country>(); } }
        public IRepository<AspNetRole> _Roles { get { return GetStandardRepo<AspNetRole>(); } }
        public IRepository<AspNetUser> _Users { get { return GetStandardRepo<AspNetUser>(); } }
        public IRepository<CategoryVideo> _CategoryVideos { get { return GetStandardRepo<CategoryVideo>(); } }
        public IRepository<UserFavoriteVideo> _Favorites { get { return GetStandardRepo<UserFavoriteVideo>(); } }
        public IRepository<UserVideoHistory> _UserVideoHistory { get { return GetStandardRepo<UserVideoHistory>(); } }
        public IRepository<SupportMessage> _SupportMessages { get { return GetStandardRepo<SupportMessage>(); } }
        public IRepository<LoggedInTracking> _LoggedInTracking { get { return GetStandardRepo<LoggedInTracking>(); } }
        public IRepository<IpAddressFilter> _IpAddressFilter { get { return GetStandardRepo<IpAddressFilter>(); } }
        public IRepository<NewFeature> _NewFeatures { get { return GetStandardRepo<NewFeature>(); } }
        public IRepository<MessageToAll> _MessageToAll { get { return GetStandardRepo<MessageToAll>(); } }
        public IRepository<VideoDownloadhistory> _VideoDownloadhistory { get { return GetStandardRepo<VideoDownloadhistory>(); } }

        #endregion

        #region Specific Repositories

        //public IOrderRepository _Orders { get { return GetRepo<IOrderRepository>(); } }

        #endregion

        #endregion

        #region Class Methods

        private void CreateDbContext()
        {
            this._DbContext = new Entities();

            // Do NOT enable proxied entities, else serialization fails
            _DbContext.Configuration.ProxyCreationEnabled = false;

            // Load navigation properties explicitly (avoid serialization trouble)
            _DbContext.Configuration.LazyLoadingEnabled = false;

            // Because We perform validation, we don't need/want EF to do so
            _DbContext.Configuration.ValidateOnSaveEnabled = false;
        }

        private IRepository<T> GetStandardRepo<T>() where T : class
        {
            return _RepositoryProvider.GetRepositoryForEntityType<T>();
        }

        private T GetRepo<T>() where T : class
        {
            return _RepositoryProvider.GetRepository<T>();
        }

        #endregion

        #region Interface Implementation

        public async Task CommitAsync()
        {
            await _DbContext.SaveChangesAsync();
        }

        #region Garbage Collector

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_DbContext != null)
                {
                    _DbContext.Dispose();
                }
            }
        }
        #endregion

        #endregion


        public int  SendMessageToAll()
        {
            return _DbContext.SendMessageToAll();
        }
    }
}
