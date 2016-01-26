﻿using System;
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

namespace DrNajeeb.Web.API.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public async Task<IHttpActionResult> GetMostWatchedVideos(int page = 1, int itemsPerPage = 20, string search = null)
        {
            try
            {
                var videosList = new List<MostWatchedVideoReportModel>();

                var videos = _Uow._UserVideoHistory.GetAll()
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
                    usersList.Add(usermodel);
                }

                // json result
                var json = new
                {
                    count = users.Count(),
                    data = usersList,
                };

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

                var modelList = new List<SubscriptionModel>();

                var revenue = await _Uow._Users.GetAll(x => x.CreatedOn >= dateFrom && x.CreatedOn <= dateTo && x.Active == true && x.IsActiveUSer == true)
                    .Include(x => x.Subscription)
                    .GroupBy(x => x.SubscriptionId)
                    .OrderBy(x=>x.Key)
                    .Select(x => new
                    {
                        TotalUsers=x.Count(),
                        SubscriptionId = x.Key
                    })
                    .ToListAsync();

                foreach (var item in revenue)
                {
                    var sub = await _Uow._Subscription.GetByIdAsync(item.SubscriptionId.Value);
                    modelList.Add(new SubscriptionModel
                    {
                        Amount=sub.Price * item.TotalUsers,
                        SubscriptionId=item.SubscriptionId.Value,
                        TotalUsers=item.TotalUsers,
                        SubscriptionName=sub.Name
                    });
                }

                var json = new
                {
                    subscriptions=modelList,
                    totalRevenue=modelList.Sum(x=>x.Amount)
                };

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        class SubscriptionModel
        {
            public string SubscriptionName { get; set; }
            public int TotalUsers { get; set; }
            public int SubscriptionId { get; set; }
            public double Amount { get; set; }
        }
    }
}