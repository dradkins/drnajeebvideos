using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DrNajeeb.Contract;
using DrNajeeb.Data;
using DrNajeeb.EF;
using System.Data.Entity;
using System.Threading.Tasks;
using DrNajeeb.Web.API.Models;
using System.Linq.Dynamic;
using System.Data.Entity.Core.Objects;
using DrNajeeb.Web.API.Helpers;
using Microsoft.AspNet.Identity;

namespace DrNajeeb.Web.API.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie)]
    public class ReportsController : BaseController
    {
        public ReportsController(IUow uow)
        {
            _Uow = uow;
        }

        [HttpGet]
        [ActionName("GetMostWatchedVideos")]
        public async Task<IHttpActionResult> GetMostWatchedVideos(DateTime dateFrom, DateTime dateTo, int page = 1, int itemsPerPage = 20, string search = null)
        {
            try
            {
                var videosList = new List<MostWatchedVideoReportModel>();

                var videos = _Uow._UserVideoHistory.GetAll(x => x.WatchDateTime >= dateFrom && x.WatchDateTime <= dateTo)
                    .Include(x => x.Video)
                    .GroupBy(x => x.VideoId)
                    .OrderByDescending(x => x.Count())
                    .Select(x => new
                    {
                        VideoId = x.Key,
                        TotalViews = x.Count(),
                        Videos = x.Where(y => y.VideoId == x.Key).FirstOrDefault().Video
                    });




                //searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    videos = videos.Where(x =>
                        x.Videos.Name.ToLower().Contains(search) ||
                        x.Videos.Description.ToLower().Contains(search));
                }

                var totalVideos = await videos.CountAsync();

                // paging
                var videosPaged = videos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);

                foreach (var video in videosPaged)
                {
                    var videoModel = new MostWatchedVideoReportModel();
                    videoModel.TotalViews = video.TotalViews;
                    videoModel.VideoName = video.Videos.Name;
                    videoModel.Duration = video.Videos.Duration;
                    videosList.Add(videoModel);
                }

                // json result
                var json = new
                {
                    count = totalVideos,
                    data = videosList,
                };

                //await LogHelpers.SaveLog(_Uow, "View Most Watched Videos Report", User.Identity.GetUserId());

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ActionName("GetVideoDownloadStats")]
        public async Task<IHttpActionResult> GetVideoDownloadStats(int page = 1, int itemsPerPage = 20, string search = null)
        {
            try
            {
                var videosList = new List<VideoDownloadHistoryModel>();

                var videos = _Uow._VideoDownloadhistory.GetAll()
                    .Include(x => x.Video)
                    .GroupBy(x => x.VideoId)
                    .OrderByDescending(x => x.Count())
                    .Select(x => new
                    {
                        VideoId = x.Key,
                        TotalDownloads = x.Count(),
                        Videos = x.Where(y => y.VideoId == x.Key).FirstOrDefault().Video
                    });




                //searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    videos = videos.Where(x =>
                        x.Videos.Name.ToLower().Contains(search) ||
                        x.Videos.Description.ToLower().Contains(search));
                }

                var totalVideos = await videos.CountAsync();

                // paging
                var videosPaged = videos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);

                foreach (var video in videosPaged)
                {
                    var videoModel = new VideoDownloadHistoryModel();
                    videoModel.TotalDownloads = video.TotalDownloads;
                    videoModel.VideoName = video.Videos.Name;
                    videoModel.Duration = video.Videos.Duration;
                    videosList.Add(videoModel);
                }

                // json result
                var json = new
                {
                    count = totalVideos,
                    data = videosList,
                };

                //await LogHelpers.SaveLog(_Uow, "View Video Download Stats Report", User.Identity.GetUserId());

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ActionName("GetUserVideoDownloadStats")]
        public async Task<IHttpActionResult> GetUserVideoDownloadStats(string userId, int page = 1, int itemsPerPage = 20, string search = null)
        {
            try
            {
                var videosList = new List<UserVideoDownloadHistoryModel>();

                var videos = _Uow._VideoDownloadhistory.GetAll(x => x.UserId == userId)
                    .Include(x => x.Video);


                var user = await _Uow._Users.GetAll(x => x.Id == userId).Select(x => x.UserName).FirstOrDefaultAsync();

                //searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    videos = videos.Where(x =>
                        x.Video.Name.ToLower().Contains(search) ||
                        x.Video.Description.ToLower().Contains(search));
                }

                var totalVideos = await videos.CountAsync();

                // paging
                var videosPaged = videos.OrderBy(x => x.Video.Name).Skip((page - 1) * itemsPerPage).Take(itemsPerPage);

                foreach (var video in videosPaged)
                {
                    var videoModel = new UserVideoDownloadHistoryModel();
                    videoModel.DateTimeDownload = video.DateTimeDownloaded;
                    videoModel.VideoName = video.Video.Name;
                    videosList.Add(videoModel);
                }

                // json result
                var json = new
                {
                    count = totalVideos,
                    data = videosList,
                    userName = user
                };

                //await LogHelpers.SaveLog(_Uow, "View User Video Download Stats Report", User.Identity.GetUserId());

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ActionName("GetUsersStatsReport")]
        public async Task<IHttpActionResult> GetUsersStatsReport(DateTime dateFrom, DateTime dateTo, int page = 1, int itemsPerPage = 20, string sortBy = "CreatedOn", bool reverse = true, string search = null, bool isActiveUser = true)
        {
            try
            {
                var usersList = new List<UserModel>();

                var users = _Uow._Users.GetAll(x => x.Active == true && x.CreatedOn >= dateFrom && x.CreatedOn <= dateTo && x.IsActiveUSer == isActiveUser)
                    .Include(x => x.Country)
                    .Include(x => x.Subscription)
                    .Include(x => x.AspNetRoles)
                    .Include(x => x.IpAddressFilters);

                // searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    users = users.Where(x =>
                        x.FullName.ToLower().Contains(search) ||
                        x.UserName.ToLower().Contains(search) ||
                        x.Email.ToLower().Contains(search));
                }

                // sorting (done with the System.Linq.Dynamic library available on NuGet)
                users = users.OrderBy(sortBy + (reverse ? " descending" : ""));

                // paging
                var usersPaged = await users.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();

                foreach (var item in usersPaged)
                {
                    var usermodel = new UserModel();
                    usermodel.CountryID = item.CountryId;
                    usermodel.EmailAddress = item.Email;
                    usermodel.FullName = item.FullName;
                    usermodel.IsActiveUser = item.IsActiveUSer;
                    usermodel.Id = item.Id;
                    usermodel.IsFilterByIP = item.IsFilterByIP;
                    usermodel.NoOfConcurrentViews = item.NoOfConcurentViews;
                    usermodel.SubscriptionID = item.SubscriptionId;
                    if (item.Country != null)
                    {
                        usermodel.Country.CountryCode = item.Country.CountryCode;
                        usermodel.Country.FlagImage = item.Country.FlagImage;
                        usermodel.Country.Name = item.Country.Name;
                    }
                    if (item.AspNetRoles != null)
                    {
                        foreach (var role in item.AspNetRoles)
                        {
                            usermodel.RolesModel.Add(new RoleModel
                            {
                                Id = role.Id,
                                Name = role.Name
                            });
                        }
                    }
                    if (item.Subscription != null)
                    {
                        usermodel.Subscription.Name = item.Subscription.Name;
                        usermodel.Subscription.Id = item.Subscription.Id;
                    }
                    if (item.IsFilterByIP)
                    {
                        if (item.IpAddressFilters != null)
                        {
                            usermodel.FilteredIPs = new List<string>();
                            foreach (var ipAddress in item.IpAddressFilters)
                            {
                                usermodel.FilteredIPs.Add(ipAddress.IpAddress);
                            }
                        }
                    }
                    usermodel.TotalVideosDownloaded = await _Uow._VideoDownloadhistory.CountAsync(x => x.UserId == item.Id);
                    usersList.Add(usermodel);
                }

                // json result
                var json = new
                {
                    count = users.Count(),
                    data = usersList,
                };

                //await LogHelpers.SaveLog(_Uow, "View User Stats Report", User.Identity.GetUserId());

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ActionName("GetRevenue")]
        public async Task<IHttpActionResult> GetRevenue(DateTime dateFrom, DateTime dateTo)
        {
            try
            {

                var modelList = new List<SubscriptionRevenueViewModel>();

                var revenue = await _Uow._Users.GetAll(x => x.CreatedOn >= dateFrom && x.CreatedOn <= dateTo && x.Active == true && x.IsActiveUSer == true)
                    .Include(x => x.Subscription)
                    .GroupBy(x => x.SubscriptionId)
                    .OrderBy(x => x.Key)
                    .Select(x => new
                    {
                        TotalUsers = x.Count(),
                        SubscriptionId = x.Key
                    })
                    .ToListAsync();

                foreach (var item in revenue)
                {
                    var sub = await _Uow._Subscription.GetByIdAsync(item.SubscriptionId.Value);
                    modelList.Add(new SubscriptionRevenueViewModel
                    {
                        Amount = sub.Price * item.TotalUsers,
                        SubscriptionId = item.SubscriptionId.Value,
                        TotalUsers = item.TotalUsers,
                        SubscriptionName = sub.Name
                    });
                }

                var json = new
                {
                    subscriptions = modelList,
                    totalRevenue = modelList.Sum(x => x.Amount)
                };

                //await LogHelpers.SaveLog(_Uow, "View Revenue Report", User.Identity.GetUserId());

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ActionName("GetMostActiveUsers")]
        public async Task<IHttpActionResult> GetMostAactiveUsers(int page = 1, int itemsPerPage = 20, string sortBy = "WatchedVideos")
        {
            try
            {
                IQueryable<MostActiveUsersViewModel> mostActiveUsers = null;
                if (sortBy == "WatchedVideos")
                {
                    mostActiveUsers = _Uow._Users.GetAll(x => x.Active == true && x.IsActiveUSer == true)
                       .OrderByDescending(x => x.UserVideoHistories.Count)
                       .ThenByDescending(x => x.VideoDownloadhistories.Count)
                       .ThenByDescending(x => x.UserFavoriteVideos.Count)
                       .Select(x => new MostActiveUsersViewModel
                       {
                           TotalVideosWatched = x.UserVideoHistories.Count,
                           TotalVideosDownloaded = x.VideoDownloadhistories.Count,
                           TotalFavoritesVideos = x.UserFavoriteVideos.Count,
                           UserName = x.UserName,
                           UserId = x.Id
                       });
                }
                else if (sortBy == "DownloadedVideos")
                {
                    mostActiveUsers = _Uow._Users.GetAll(x => x.Active == true && x.IsActiveUSer == true)
                        .OrderByDescending(x => x.VideoDownloadhistories.Count)
                       .ThenByDescending(x => x.UserVideoHistories.Count)
                       .ThenByDescending(x => x.UserFavoriteVideos.Count)
                       .Select(x => new MostActiveUsersViewModel
                       {
                           TotalVideosWatched = x.UserVideoHistories.Count,
                           TotalVideosDownloaded = x.VideoDownloadhistories.Count,
                           TotalFavoritesVideos = x.UserFavoriteVideos.Count,
                           UserName = x.UserName,
                           UserId = x.Id
                       });
                }
                else if (sortBy == "FavoriteVideos")
                {
                    mostActiveUsers = _Uow._Users.GetAll(x => x.Active == true && x.IsActiveUSer == true)
                        .OrderByDescending(x => x.UserFavoriteVideos.Count)
                        .ThenByDescending(x => x.VideoDownloadhistories.Count)
                       .ThenByDescending(x => x.UserVideoHistories.Count)
                       .Select(x => new MostActiveUsersViewModel
                       {
                           TotalVideosWatched = x.UserVideoHistories.Count,
                           TotalVideosDownloaded = x.VideoDownloadhistories.Count,
                           TotalFavoritesVideos = x.UserFavoriteVideos.Count,
                           UserName = x.UserName,
                           UserId = x.Id
                       });
                }
                var totalUsers = await mostActiveUsers.CountAsync();

                // paging
                var usersPaged = mostActiveUsers.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);

                List<MostActiveUsersViewModel> usersList = new List<MostActiveUsersViewModel>();

                foreach (var item in usersPaged)
                {
                    usersList.Add(new MostActiveUsersViewModel
                    {
                        TotalVideosWatched = item.TotalVideosWatched,
                        TotalVideosDownloaded = item.TotalVideosDownloaded,
                        TotalFavoritesVideos = item.TotalFavoritesVideos,
                        UserName = item.UserName,
                        UserId = item.UserId
                    });
                }

                var json = new
                {
                    count = totalUsers,
                    data = usersList,
                };

                //await LogHelpers.SaveLog(_Uow, "View Most Active Users Report", User.Identity.GetUserId());

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ActionName("GetGhostUsers")]
        public async Task<IHttpActionResult> GetMostAactiveUsers(DateTime dateFrom, int page = 1, int itemsPerPage = 20, string sortBy = "CreatedOn", bool reverse = true, string search = null)
        {
            try
            {

                using (var context = new DrNajeeb.EF.Entities())
                {
                    IQueryable<AspNetUser> usersList = null;

                    usersList = context.AspNetUsers
                        .Where(x => x.IsActiveUSer == true && x.Active == true)
                        .Where(user => !context.UserVideoHistories
                            .Any(f => f.UserId == user.Id && f.WatchDateTime > dateFrom) && !context.VideoDownloadhistories
                            .Any(f => f.UserId == user.Id && f.DateTimeDownloaded > dateFrom))
                            .Include(x => x.Country)
                        .Include(x => x.Subscription)
                        .Include(x => x.AspNetRoles)
                        .Include(x => x.IpAddressFilters);

                    // searching
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        search = search.ToLower();
                        usersList = usersList.Where(x =>
                            x.FullName.ToLower().Contains(search) ||
                            x.UserName.ToLower().Contains(search) ||
                            x.Email.ToLower().Contains(search));
                    }


                    var totalUsers = await usersList.CountAsync();

                    // sorting (done with the System.Linq.Dynamic library available on NuGet)
                    usersList = usersList.OrderBy(sortBy + (reverse ? " descending" : ""));

                    // paging
                    var usersPaged = usersList.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);

                    var users = new List<UserModel>();

                    foreach (var item in usersPaged)
                    {
                        var usermodel = new UserModel();
                        usermodel.CountryID = item.CountryId;
                        usermodel.EmailAddress = item.Email;
                        usermodel.FullName = item.FullName;
                        usermodel.IsActiveUser = item.IsActiveUSer;
                        usermodel.Id = item.Id;
                        usermodel.IsFilterByIP = item.IsFilterByIP;
                        usermodel.NoOfConcurrentViews = item.NoOfConcurentViews;
                        usermodel.SubscriptionID = item.SubscriptionId;
                        if (item.Country != null)
                        {
                            usermodel.Country.CountryCode = item.Country.CountryCode;
                            usermodel.Country.FlagImage = item.Country.FlagImage;
                            usermodel.Country.Name = item.Country.Name;
                        }
                        if (item.AspNetRoles != null)
                        {
                            foreach (var role in item.AspNetRoles)
                            {
                                usermodel.RolesModel.Add(new RoleModel
                                {
                                    Id = role.Id,
                                    Name = role.Name
                                });
                            }
                        }
                        if (item.Subscription != null)
                        {
                            usermodel.Subscription.Name = item.Subscription.Name;
                            usermodel.Subscription.Id = item.Subscription.Id;
                        }
                        if (item.IsFilterByIP)
                        {
                            if (item.IpAddressFilters != null)
                            {
                                usermodel.FilteredIPs = new List<string>();
                                foreach (var ipAddress in item.IpAddressFilters)
                                {
                                    usermodel.FilteredIPs.Add(ipAddress.IpAddress);
                                }
                            }
                        }
                        users.Add(usermodel);
                    }

                    // json result
                    var json = new
                    {
                        count = totalUsers,
                        data = users,
                    };

                    //await LogHelpers.SaveLog(_Uow, "View Ghost Users Report", User.Identity.GetUserId());

                    return Ok(json);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [ActionName("GetUserActivityReport")]
        public async Task<IHttpActionResult> GetUserActivityReport(string userId, int page = 1, int itemsPerPage = 20)
        {
            try
            {

                List<UserActivityLogModel> userActivity = new List<UserActivityLogModel>();

                var userDownloads = _Uow._VideoDownloadhistory.GetAll(x => x.UserId == userId).Include(x => x.Video);
                var userWatchedVideos = _Uow._UserVideoHistory.GetAll(x => x.UserId == userId).Include(x => x.Video);

                foreach (var item in userDownloads)
                {
                    userActivity.Add(new UserActivityLogModel
                    {
                        ActionName = "User Download ",
                        DateTimeAction = item.DateTimeDownloaded,
                        VideoName = item.Video.Name
                    });
                }

                foreach (var item in userWatchedVideos)
                {
                    userActivity.Add(new UserActivityLogModel
                    {
                        ActionName = "User Watch ",
                        DateTimeAction = item.WatchDateTime,
                        VideoName = item.Video.Name
                    });
                }

                var activities = userActivity.OrderBy(x => x.DateTimeAction);
                var userName = _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefault().UserName;

                var json = new
                {
                    data = activities,
                    userName = userName
                };

                //await LogHelpers.SaveLog(_Uow, "View User Activity Report of " + userName, User.Identity.GetUserId());

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetManagerActivityReport")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetManagerActivityReport(string userId, int page = 1, int itemsPerPage = 20)
        {
            try
            {
                List<UserActivityLogModel> userActivity = new List<UserActivityLogModel>();

                var managerActivities = _Uow._ManagerLog.GetAll(x=>x.UserId==userId);

                foreach (var item in managerActivities)
                {
                    userActivity.Add(new UserActivityLogModel
                    {
                        ActionName = item.EventDetails,
                        DateTimeAction = item.EventDateTime,
                    });
                }

                var activities = userActivity.OrderByDescending(x => x.DateTimeAction);
                var userName = _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefault().UserName;

                var json = new
                {
                    data = activities,
                    userName = userName
                };

                //await LogHelpers.SaveLog(_Uow, "View Manager Activity Report of " + userName, User.Identity.GetUserId());

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        class UserActivityLogModel
        {
            public string ActionName { get; set; }
            public DateTime DateTimeAction { get; set; }
            public string VideoName { get; set; }
        }
    }
}
