using DrNajeeb.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrNajeeb.Contract
{
    public interface IUow : IDisposable
    {
        #region Methods

        Task CommitAsync();

        #endregion

        #region Repositories

        IRepository<Subscription> _Subscription { get; }
        IRepository<Category> _Categories { get; }
        IRepository<Video> _Videos { get; }
        IRepository<Country> _Country { get; }
        IRepository<AspNetRole> _Roles { get; }
        IRepository<AspNetUser> _Users { get; }
        IRepository<CategoryVideo> _CategoryVideos { get; }
        IRepository<UserFavoriteVideo> _Favorites { get; }
        IRepository<UserVideoHistory> _UserVideoHistory { get; }
        IRepository<SupportMessage> _SupportMessages { get; }

        #endregion
    }
}
