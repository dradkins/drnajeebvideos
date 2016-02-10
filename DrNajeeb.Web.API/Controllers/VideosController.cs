using DrNajeeb.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Linq.Dynamic;
using DrNajeeb.Web.API.Models;
using System.Threading.Tasks;
using DrNajeeb.EF;
using Microsoft.AspNet.Identity;
using DrNajeeb.Web.API.Helpers;

namespace DrNajeeb.Web.API.Controllers
{
    [Authorize]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie)]

    public class VideosController : BaseController
    {

        public VideosController(IUow uow)
        {
            _Uow = uow;
        }

        [ActionName("GetAll")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> GetAll(int page = 1, int itemsPerPage = 20, string sortBy = "Name", bool reverse = false, string search = null)
        {
            try
            {
                var videosList = new List<VideoModel>();

                var videos = _Uow._Videos.GetAll(x => x.Active == true)
                    .Include(x => x.CategoryVideos);

                // searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    videos = videos.Where(x =>
                        x.Name.ToLower().Contains(search) ||
                        x.Description.ToLower().Contains(search));
                }

                var totalVideos = await videos.CountAsync();

                // sorting (done with the System.Linq.Dynamic library available on NuGet)
                videos = videos.OrderBy(sortBy + (reverse ? " descending" : ""));

                // paging
                var videosPaged = videos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);

                foreach (var video in videosPaged)
                {
                    var model = new VideoModel();
                    model.Id = video.Id;
                    model.Name = video.Name;
                    model.Description = video.Description;
                    model.Duration = video.Duration;
                    model.ReleaseYear = video.ReleaseYear;
                    if (video.DateLive != null)
                    {
                        model.DateLive = video.DateLive;
                    }
                    model.BackgroundColor = video.BackgroundColor;
                    model.IsEnabled = video.IsEnabled;
                    model.StandardVideoId = video.StandardVideoId;
                    model.FastVideoId = video.StandardVideoId;
                    if (video.CategoryVideos != null && video.CategoryVideos.Count > 0)
                    {
                        foreach (var item in video.CategoryVideos)
                        {
                            model.Categories.Add(item.CategoryId.Value);
                        }
                    }
                    videosList.Add(model);
                }

                // json result
                var json = new
                {
                    count = totalVideos,
                    data = videosList,
                };

                await LogHelpers.SaveLog(_Uow, "Check All Videos", User.Identity.GetUserId());

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [ActionName("GetByCategoryForSorting")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> GetByCategoryForSorting(int id)
        {
            try
            {
                var videosList = new List<VideoModel>();

                var videos = _Uow._CategoryVideos
                    .GetAll(x => x.CategoryId == id)
                    .OrderBy(x => x.DisplayOrder)
                    .Include(x => x.Video)
                    .Select(x => x.Video);

                foreach (var video in videos)
                {
                    var model = new VideoModel();
                    model.Id = video.Id;
                    model.Name = video.Name;
                    model.Description = video.Description;
                    model.Duration = video.Duration;
                    model.ReleaseYear = video.ReleaseYear;
                    if (video.DateLive != null)
                    {
                        model.DateLive = video.DateLive;
                    }
                    model.BackgroundColor = video.BackgroundColor;
                    model.IsEnabled = video.IsEnabled;
                    model.StandardVideoId = video.StandardVideoId;
                    model.FastVideoId = video.StandardVideoId;
                    videosList.Add(model);
                }

                await LogHelpers.SaveLog(_Uow, "Get Videos Of Category For Sorting", User.Identity.GetUserId());

                return Ok(videosList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [ActionName("UpdateOrder")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> UpdateOrder(List<VideoSortingModel> model)
        {
            try
            {
                var categoryId = model.First().CategoryId;
                var categoryVideos = _Uow._CategoryVideos.GetAll(x => x.CategoryId == categoryId);

                foreach (var item in model)
                {
                    var displayOrder = item.LocationNo + 1;
                    var categoryVideo = await categoryVideos.FirstOrDefaultAsync(x => x.VideoId == item.VideoId);
                    if (categoryVideo != null)
                    {
                        if (categoryVideo.DisplayOrder == null || categoryVideo.DisplayOrder != displayOrder)
                        {
                            categoryVideo.DisplayOrder = displayOrder;
                            _Uow._CategoryVideos.Update(categoryVideo);
                        }
                    }
                }
                await _Uow.CommitAsync();

                await LogHelpers.SaveLog(_Uow, "Update Soring Of Videos", User.Identity.GetUserId());

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("AddVideo")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> AddVideo(VideoModel model)
        {
            try
            {
                var video = new DrNajeeb.EF.Video();
                video.Name = model.Name;
                video.Description = model.Description;
                video.Duration = model.Duration;
                video.ReleaseYear = model.ReleaseYear;
                if (video.DateLive != null)
                {
                    video.DateLive = model.DateLive.Value;
                }
                else
                {
                    video.DateLive = DateTime.UtcNow;
                }
                video.BackgroundColor = model.BackgroundColor;
                video.IsEnabled = model.IsEnabled;
                video.StandardVideoId = model.StandardVideoId;
                video.FastVideoId = model.FastVideoId;
                video.Active = true;
                video.CreatedOn = DateTime.UtcNow;
                video.IsFreeVideo = model.IsFreeVideo;
                video.ThumbnailURL = @"http://view.vzaar.com/" + model.StandardVideoId + "/thumb";

                if (model.Categories != null && model.Categories.Count > 0)
                {
                    foreach (var item in model.Categories)
                    {
                        video.CategoryVideos.Add(new EF.CategoryVideo
                        {
                            CategoryId = item,
                            VideoId = video.Id,
                            CreatedOn = DateTime.UtcNow,
                        });
                    }
                }

                //todo : add useid in createdby

                _Uow._Videos.Add(video);
                await _Uow.CommitAsync();
                var json = new
                {
                    Id = video.Id,
                    Name = video.Name
                };

                await LogHelpers.SaveLog(_Uow, "Add Video : "+video.Name, User.Identity.GetUserId());

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("DeleteVideo")]
        [HttpDelete]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> DeleteVideo(int id)
        {
            try
            {
                var video = await _Uow._Videos.GetByIdAsync(id);
                if (video == null)
                {
                    return NotFound();
                }

                video.Active = false;
                video.UpdatedOn = DateTime.Now;
                _Uow._Videos.Update(video);

                var categoryVideos = _Uow._CategoryVideos.GetAll(x => x.VideoId == id);
                foreach (var item in categoryVideos)
                {
                    _Uow._CategoryVideos.Delete(item);
                }
                video.UpdatedBy = User.Identity.GetUserId();

                await LogHelpers.SaveLog(_Uow, "Delete Video : " + video.Name, User.Identity.GetUserId());

                await _Uow.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("UpdateVideo")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> UpdateVideo(VideoModel model)
        {
            try
            {
                var video = await _Uow._Videos.GetByIdAsync(model.Id);
                video.Name = model.Name;
                video.Description = model.Description;
                video.Duration = model.Duration;
                video.ReleaseYear = model.ReleaseYear;
                if (video.DateLive != null)
                {
                    video.DateLive = model.DateLive.Value;
                }
                video.BackgroundColor = model.BackgroundColor;
                video.IsEnabled = model.IsEnabled;
                video.StandardVideoId = model.StandardVideoId;
                video.FastVideoId = model.StandardVideoId;
                video.IsFreeVideo = model.IsFreeVideo ?? false;
                if (model.Categories != null && model.Categories.Count > 0)
                {
                    foreach (var item in model.Categories)
                    {
                        video.CategoryVideos.Add(new EF.CategoryVideo
                        {
                            CategoryId = item,
                            VideoId = video.Id,
                            CreatedOn = DateTime.UtcNow
                        });
                    }
                }

                var categoryVideos = _Uow._CategoryVideos.GetAll(x => x.VideoId == model.Id);
                foreach (var item in categoryVideos)
                {
                    _Uow._CategoryVideos.Delete(item);
                }
                video.UpdatedBy = User.Identity.GetUserId();

                _Uow._Videos.Update(video);
                await _Uow.CommitAsync();

                await LogHelpers.SaveLog(_Uow, "Update Video : " + video.Name, User.Identity.GetUserId());

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetSingle")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> GetSingle(int Id)
        {
            try
            {
                var video = await _Uow._Videos.GetByIdAsync(Id);
                if (video == null)
                {
                    return NotFound();
                }

                var model = new VideoModel();
                model.Name = video.Name;
                model.Description = video.Description;
                model.Duration = video.Duration;
                model.ReleaseYear = video.ReleaseYear;
                if (model.DateLive != null)
                {
                    model.DateLive = video.DateLive;
                }
                model.BackgroundColor = video.BackgroundColor;
                model.IsEnabled = video.IsEnabled;
                model.StandardVideoId = video.StandardVideoId;
                model.FastVideoId = video.StandardVideoId;

                await LogHelpers.SaveLog(_Uow, "Check Single Video : " + video.Name, User.Identity.GetUserId());

                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetVideosCount")]
        [Authorize]
        public async Task<IHttpActionResult> GetVideosCount()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var _User = _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefault();
                if (_User.IsFreeUser != null && _User.IsFreeUser.Value)
                {
                    var videosCount = await _Uow._Videos.CountAsync(x => x.Active == true && x.IsFreeVideo.Value);
                    return Ok(videosCount);
                }

                var videoscount = await _Uow._Videos.CountAsync(x => x.Active == true);

                return Ok(videoscount);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [ActionName("GetByCategory")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetByCategory(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var _User = _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefault();
                var videosList = new List<UserVideoModel>();
                var videos = _Uow._CategoryVideos
                    .GetAll(x => x.CategoryId == id)
                    .OrderBy(x => x.DisplayOrder)
                    .Include(x => x.Video)
                    .Select(x => x.Video)
                    .Include(x => x.UserFavoriteVideos);

                //if (_User.IsFreeUser != null && _User.IsFreeUser.Value)
                //{
                //    videos = videos.Where(x => x.IsFreeVideo.Value);
                //}

                var videoslist = await videos.ToListAsync();

                videoslist.ForEach(x => videosList.Add(new UserVideoModel
                {
                    BackgroundColor = x.BackgroundColor,
                    DateLive = x.DateLive,
                    Duration = x.Duration,
                    Id = x.Id,
                    Name = x.Name,
                    ReleaseYear = x.ReleaseYear,
                    VzaarVideoId = x.StandardVideoId,
                    ThumbnailURL = x.ThumbnailURL,
                    IsFavoriteVideo = (x.UserFavoriteVideos != null && x.UserFavoriteVideos.Any(y => y.VideoId == x.Id && y.UserId == userId)),
                }));
                return Ok(videosList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetUserVideo")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetUserVideo(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var _User = _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefault();

                var videosModel = new UserVideoModel();
                var video = await _Uow._Videos.GetAll(x => x.Id == id)
                    .Include(x => x.CategoryVideos)
                    .Include(x => x.CategoryVideos.Select(y => y.Category))
                    .Include(x => x.UserFavoriteVideos)
                    .FirstOrDefaultAsync();

                if (video == null)
                {
                    return NotFound();
                }

                if (_User.IsFreeUser.Value)
                {
                    if (!video.IsFreeVideo.Value)
                    {
                        return BadRequest();
                    }
                }

                videosModel.BackgroundColor = video.BackgroundColor;
                videosModel.DateLive = video.DateLive;
                videosModel.Duration = video.Duration;
                videosModel.Id = video.Id;
                videosModel.Name = video.Name;
                videosModel.ReleaseYear = video.ReleaseYear;
                videosModel.VzaarVideoId = video.StandardVideoId;
                videosModel.ThumbnailURL = video.ThumbnailURL;
                videosModel.Description = video.Description;
                videosModel.TotalViews = await _Uow._UserVideoHistory.CountAsync(x => x.VideoId == video.Id);
                if (video.UserFavoriteVideos != null && video.UserFavoriteVideos.Count > 0)
                {
                    videosModel.IsFavoriteVideo = video.UserFavoriteVideos.Any(x => x.VideoId == id && x.UserId == userId);
                }
                if (video.CategoryVideos != null && video.CategoryVideos.Count > 0)
                {
                    foreach (var item in video.CategoryVideos)
                    {
                        videosModel.Categories.Add(new UserCategoryModel
                        {
                            Id = item.Category.Id,
                            Name = item.Category.Name
                        });
                    }

                }


                var lastWatched = await _Uow._UserVideoHistory
                    .GetAll(x => x.VideoId == id && x.UserId == userId)
                    .OrderByDescending(x => x.WatchDateTime)
                    .FirstOrDefaultAsync();

                if (lastWatched != null)
                {
                    videosModel.LastSeekTime = lastWatched.LastSeekTime.GetValueOrDefault();
                }

                //add video to user history
                var userHistory = new UserVideoHistory();
                userHistory.UserId = userId;
                userHistory.VideoId = id;
                userHistory.WatchDateTime = DateTime.UtcNow;
                _Uow._UserVideoHistory.Add(userHistory);
                await _Uow.CommitAsync();

                videosModel.WatchedVideoId = userHistory.Id;

                return Ok(videosModel);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("SaveVideoTime")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> SaveVideoTime(SaveVideoTimeViewModel model)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var video = await _Uow._UserVideoHistory.GetAsync(x => x.Id == model.WatchedVideoId && x.UserId == userId);
                if (video != null)
                {
                    video.LastSeekTime = model.CurrentTime;
                    _Uow._UserVideoHistory.Update(video);
                    await _Uow.CommitAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("getUserNewVideo")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetUserNewVideo(int page = 1, int itemsPerPage = 20, string search = null)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var _User = _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefault();
                var videosList = new List<UserVideoModel>();

                var videos = _Uow._Videos.GetAll(x => x.Active == true)
                    .Include(x => x.UserFavoriteVideos)
                    .Include(x => x.CategoryVideos)
                    .Include(x => x.CategoryVideos.Select(y => y.Category));

                // searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    videos = videos.Where(x =>
                        x.Name.ToLower().Contains(search) ||
                        x.Description.ToLower().Contains(search));
                }

                videos = videos.OrderBy("DateLive descending");

                int totalVideos = 0;
                totalVideos = await videos.CountAsync();

                //if (_User == null)
                //{
                //    return InternalServerError();
                //}

                //if (_User.IsFreeUser != null && _User.IsFreeUser.Value)
                //{
                //    videos = videos.Where(x => x.IsFreeVideo.Value);
                //    totalVideos = await videos.CountAsync(x => x.IsFreeVideo.Value);
                //}

                // paging
                var videosPaged = await videos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();

                videosPaged.ForEach(x =>
                {
                    var uvm = new UserVideoModel();
                    uvm.BackgroundColor = x.BackgroundColor;
                    uvm.DateLive = x.DateLive;
                    uvm.Duration = x.Duration;
                    uvm.Id = x.Id;
                    uvm.Name = x.Name;
                    uvm.ReleaseYear = x.ReleaseYear;
                    uvm.VzaarVideoId = x.StandardVideoId;
                    uvm.ThumbnailURL = x.ThumbnailURL;
                    uvm.IsFavoriteVideo = (x.UserFavoriteVideos != null && x.UserFavoriteVideos.Any(y => y.VideoId == x.Id && y.UserId == userId));
                    if (x.CategoryVideos != null && x.CategoryVideos.Count > 0)
                    {
                        x.CategoryVideos.ToList().ForEach(y =>
                        {
                            uvm.Categories.Add(new UserCategoryModel
                            {
                                Id = y.Category.Id,
                                Name = y.Category.Name
                            });
                        });
                    }
                    videosList.Add(uvm);
                });

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

        [ActionName("getUserFreeVideo")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetUserFreeVideo(int page = 1, int itemsPerPage = 20, string search = null)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var _User = _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefault();
                var videosList = new List<UserVideoModel>();

                var videos = _Uow._Videos.GetAll(x => x.Active == true && x.IsFreeVideo == true)
                    .Include(x => x.UserFavoriteVideos)
                    .Include(x => x.CategoryVideos)
                    .Include(x => x.CategoryVideos.Select(y => y.Category));

                // searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    videos = videos.Where(x =>
                        x.Name.ToLower().Contains(search) ||
                        x.Description.ToLower().Contains(search));
                }

                videos = videos.OrderBy("DateLive");

                int totalVideos = 0;
                totalVideos = await videos.CountAsync();

                // paging
                var videosPaged = await videos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();

                videosPaged.ForEach(x =>
                {
                    var uvm = new UserVideoModel();
                    uvm.BackgroundColor = x.BackgroundColor;
                    uvm.DateLive = x.DateLive;
                    uvm.Duration = x.Duration;
                    uvm.Id = x.Id;
                    uvm.Name = x.Name;
                    uvm.ReleaseYear = x.ReleaseYear;
                    uvm.VzaarVideoId = x.StandardVideoId;
                    uvm.ThumbnailURL = x.ThumbnailURL;
                    uvm.IsFavoriteVideo = (x.UserFavoriteVideos != null && x.UserFavoriteVideos.Any(y => y.VideoId == x.Id && y.UserId == userId));
                    if (x.CategoryVideos != null && x.CategoryVideos.Count > 0)
                    {
                        x.CategoryVideos.ToList().ForEach(y =>
                        {
                            uvm.Categories.Add(new UserCategoryModel
                            {
                                Id = y.Category.Id,
                                Name = y.Category.Name
                            });
                        });
                    }
                    videosList.Add(uvm);
                });

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

        [ActionName("AddUserVideoToFavorite")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> AddUserVideoToFavorite(UserFavoriteVideoModel model)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                if (_Uow._Favorites.GetAll(x => x.VideoId == model.VideoId && x.UserId == userId).Any())
                {
                    return Ok();
                }
                _Uow._Favorites.Add(new EF.UserFavoriteVideo
                {
                    UserId = userId,
                    VideoId = model.VideoId
                });
                await _Uow.CommitAsync();
                return Ok(model.VideoId);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("DeleteUserVideoToFavorite")]
        [HttpDelete]
        [Authorize]
        public async Task<IHttpActionResult> DeleteUserVideoToFavorite(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var _User = _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefault();
                var favoriteVideo = await _Uow._Favorites.GetAsync(x => x.UserId == userId && x.VideoId == id);
                if (favoriteVideo == null)
                {
                    return NotFound();
                }
                _Uow._Favorites.Delete(favoriteVideo);
                await _Uow.CommitAsync();
                return Ok(id);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetUserFavoriteVideos")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetUserFavoriteVideos(int page = 1, int itemsPerPage = 20, string search = null)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var _User = _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefault();
                var videosList = new List<UserVideoModel>();

                var videos = _Uow._Favorites.GetAll(x => x.UserId == userId)
                    .Include(x => x.Video)
                    .Select(x => x.Video)
                    .Include(x => x.CategoryVideos)
                    .Include(x => x.CategoryVideos.Select(y => y.Category));

                // searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    videos = videos.Where(x =>
                        x.Name.ToLower().Contains(search) ||
                        x.Description.ToLower().Contains(search));
                }

                videos = videos.OrderBy("DateLive");

                int totalVideos = 0;
                totalVideos = await videos.CountAsync();

                //if (_User.IsFreeUser != null && _User.IsFreeUser.Value)
                //{
                //    videos = videos.Where(x => x.IsFreeVideo.Value);
                //    totalVideos = await videos.CountAsync(x => x.IsFreeVideo.Value);
                //}

                // paging
                var videosPaged = await videos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();

                videosPaged.ForEach(x =>
                {
                    var uvm = new UserVideoModel();
                    uvm.BackgroundColor = x.BackgroundColor;
                    uvm.DateLive = x.DateLive;
                    uvm.Duration = x.Duration;
                    uvm.Id = x.Id;
                    uvm.Name = x.Name;
                    uvm.ReleaseYear = x.ReleaseYear;
                    uvm.VzaarVideoId = x.StandardVideoId;
                    uvm.ThumbnailURL = x.ThumbnailURL;
                    uvm.IsFavoriteVideo = (x.UserFavoriteVideos != null && x.UserFavoriteVideos.Any(y => y.VideoId == x.Id));
                    if (x.CategoryVideos != null && x.CategoryVideos.Count > 0)
                    {
                        x.CategoryVideos.ToList().ForEach(y =>
                        {
                            uvm.Categories.Add(new UserCategoryModel
                            {
                                Id = y.Category.Id,
                                Name = y.Category.Name
                            });
                        });
                    }
                    videosList.Add(uvm);
                });

                //var count = await _Uow._Favorites.CountAsync(x => x.UserId == userId);

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

        [ActionName("GetUserDownloadedVideos")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetUserDownloadedVideos(int page = 1, int itemsPerPage = 20, string search = null)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var _User = _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefault();
                var videosList = new List<UserVideoModel>();

                var videos = _Uow._VideoDownloadhistory.GetAll(x => x.UserId == userId)
                    .Include(x => x.Video)
                    .Select(x => x.Video)
                    .Include(x => x.CategoryVideos)
                    .Include(x => x.CategoryVideos.Select(y => y.Category));

                // searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    videos = videos.Where(x =>
                        x.Name.ToLower().Contains(search) ||
                        x.Description.ToLower().Contains(search));
                }

                videos = videos.OrderBy("DateLive");

                int totalVideos = 0;
                totalVideos = await videos.CountAsync();

                // paging
                var videosPaged = await videos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();

                videosPaged.ForEach(x =>
                {
                    var uvm = new UserVideoModel();
                    uvm.BackgroundColor = x.BackgroundColor;
                    uvm.DateLive = x.DateLive;
                    uvm.Duration = x.Duration;
                    uvm.Id = x.Id;
                    uvm.Name = x.Name;
                    uvm.ReleaseYear = x.ReleaseYear;
                    uvm.VzaarVideoId = x.StandardVideoId;
                    uvm.ThumbnailURL = x.ThumbnailURL;
                    if (x.CategoryVideos != null && x.CategoryVideos.Count > 0)
                    {
                        x.CategoryVideos.ToList().ForEach(y =>
                        {
                            uvm.Categories.Add(new UserCategoryModel
                            {
                                Id = y.Category.Id,
                                Name = y.Category.Name
                            });
                        });
                    }
                    videosList.Add(uvm);
                });

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

        [ActionName("GetVideoTotalDownloads")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetVideoTotalDownloads(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var totalDownloads = await _Uow._VideoDownloadhistory.CountAsync(x => x.VideoId == id && x.UserId == userId);
                return Ok(totalDownloads);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [ActionName("GetUserVideosHistory")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetUserVideosHistory(int page = 1, int itemsPerPage = 20, string search = null)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var _User = _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefault();
                var videosList = new List<UserVideoModel>();

                var videos = _Uow._UserVideoHistory.GetAll(x => x.UserId == userId)
                    .OrderByDescending(x => x.WatchDateTime)
                    .Include(x => x.Video)
                    .Select(x => x.Video)
                    .Include(x => x.CategoryVideos)
                    .Include(x => x.CategoryVideos.Select(y => y.Category));

                // searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    videos = videos.Where(x =>
                        x.Name.ToLower().Contains(search) ||
                        x.Description.ToLower().Contains(search));
                }

                int totalVideos = 0;
                totalVideos = await videos.CountAsync();

                //if (_User.IsFreeUser != null && _User.IsFreeUser.Value)
                //{
                //    videos = videos.Where(x => x.IsFreeVideo.Value);
                //    totalVideos = await videos.CountAsync(x => x.IsFreeVideo.Value);
                //}

                // paging
                var videosPaged = await videos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();

                videosPaged.ForEach(x =>
                {
                    var uvm = new UserVideoModel();
                    uvm.BackgroundColor = x.BackgroundColor;
                    uvm.DateLive = x.DateLive;
                    uvm.Duration = x.Duration;
                    uvm.Id = x.Id;
                    uvm.Name = x.Name;
                    uvm.ReleaseYear = x.ReleaseYear;
                    uvm.VzaarVideoId = x.StandardVideoId;
                    uvm.ThumbnailURL = x.ThumbnailURL;
                    uvm.IsFavoriteVideo = (x.UserFavoriteVideos != null && x.UserFavoriteVideos.Any(y => y.VideoId == x.Id));
                    if (x.CategoryVideos != null && x.CategoryVideos.Count > 0)
                    {
                        x.CategoryVideos.ToList().ForEach(y =>
                        {
                            uvm.Categories.Add(new UserCategoryModel
                            {
                                Id = y.Category.Id,
                                Name = y.Category.Name
                            });
                        });
                    }
                    videosList.Add(uvm);
                });

                //var count = await _Uow._UserVideoHistory.CountAsync(x => x.UserId == userId);

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

        [ActionName("GetByCategoryFrontEnd")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetByCategoryFrontEnd(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var _User = _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefault();
                var videosList = new List<UserVideoModel>();
                var videos = _Uow._CategoryVideos
                    .GetAll(x => x.CategoryId == id)
                    .Include(x => x.Video)
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => x.Video);

                //if (_User.IsFreeUser != null && _User.IsFreeUser.Value)
                //{
                //    videos = videos.Where(x => x.IsFreeVideo == true);
                //}

                var videosToList = await videos.ToListAsync();

                videosToList.ForEach(x => videosList.Add(new UserVideoModel
                {
                    Name = x.Name,
                    ThumbnailURL = x.ThumbnailURL,
                    VzaarVideoId = x.StandardVideoId
                }));
                return Ok(videosList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("SearchVideos")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> SearchVideos(string id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var _User = _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefault();
                if (!string.IsNullOrEmpty(id))
                {
                    if (_User.IsFreeUser != null && _User.IsFreeUser.Value)
                    {
                        var videos = await _Uow._Videos.GetAll(x => x.Name.Contains(id) && x.IsFreeVideo == true).Select(x => x.Name).ToListAsync();
                        return Ok(videos);
                    }

                    var vides = await _Uow._Videos.GetAll(x => x.Name.Contains(id)).Select(x => x.Name).ToListAsync();
                    return Ok(vides);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("VideoNotifications")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> VideoNotifications()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var user = await _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefaultAsync();

                var dateAfter = Convert.ToDateTime("10/29/2015 11:08:24 AM");
                var videosList = new List<UserVideoModel>();

                var unWatchedVideos = _Uow._Videos.GetAll(x => x.CreatedOn > dateAfter)
                    .Include(x => x.UserVideoHistories)
                    .Where(x => x.UserVideoHistories.Any(y => y.UserId == userId))
                    .Where(x => !x.UserVideoHistories.Any(y => y.Id == x.Id));

                var totalVideos = await unWatchedVideos.CountAsync();
                var notificationVideos = await unWatchedVideos.OrderBy(x => x.CreatedOn).Take(10).ToListAsync();

                foreach (var item in notificationVideos)
                {
                    videosList.Add(new UserVideoModel
                    {
                        Id = item.Id,
                        Name = item.Name
                    });
                }
                return Ok(new { ToNotificationVideos = totalVideos, VideosList = videosList });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("SaveDownloadStats")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> SaveDownloadStats(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                _Uow._VideoDownloadhistory.Add(new VideoDownloadhistory
                {
                    DateTimeDownloaded = DateTime.UtcNow,
                    UserId = userId,
                    VideoId = id
                });
                await _Uow.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetWeeklyTopVideos")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetWeeklyTopVideos()
        {
            try
            {
                var videosList = new List<UserVideoModel>();
                var minDate = DateTime.UtcNow.AddDays(-7);

                var videos = await _Uow._Videos.GetAll(x => x.Active == true)
                    .Include(x => x.UserVideoHistories)
                    .Select(x => new
                    {
                        video = x,
                        totalViews = x.UserVideoHistories.Count(y => y.WatchDateTime <= DateTime.UtcNow && y.WatchDateTime >= minDate)
                    })
                    .OrderByDescending(x => x.totalViews)
                    .Select(x => x.video)
                    .Include(x => x.CategoryVideos)
                    .Include(x=>x.CategoryVideos.Select(y=>y.Category))
                    .Take(5)
                    .ToListAsync();

                videos.ForEach(x =>
                {
                    var uvm = new UserVideoModel();
                    uvm.BackgroundColor = x.BackgroundColor;
                    uvm.DateLive = x.DateLive;
                    uvm.Duration = x.Duration;
                    uvm.Id = x.Id;
                    uvm.Name = x.Name;
                    uvm.ReleaseYear = x.ReleaseYear;
                    uvm.VzaarVideoId = x.StandardVideoId;
                    uvm.ThumbnailURL = x.ThumbnailURL;
                    if (x.CategoryVideos != null && x.CategoryVideos.Count > 0)
                    {
                        x.CategoryVideos.ToList().ForEach(y =>
                        {
                            uvm.Categories.Add(new UserCategoryModel
                            {
                                Id = y.Category.Id,
                                Name = y.Category.Name
                            });
                        });
                    }
                    videosList.Add(uvm);
                });

                // json result
                var json = new
                {
                    count = 5,
                    data = videosList,
                };

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [ActionName("GetAllTimeTopVideos")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetAllTimeTopVideos()
        {
            try
            {
                var videosList = new List<UserVideoModel>();

                var videos = await _Uow._Videos.GetAll(x => x.Active == true)
                    .Include(x => x.UserVideoHistories)
                    .Select(x => new
                    {
                        video = x,
                        totalViews = x.UserVideoHistories.Count
                    })
                    .OrderByDescending(x => x.totalViews)
                    .Select(x => x.video)
                    .Include(x=>x.CategoryVideos)
                    .Include(x => x.CategoryVideos.Select(y => y.Category))
                    .Take(5)
                    .ToListAsync();

                videos.ForEach(x =>
                {
                    var uvm = new UserVideoModel();
                    uvm.BackgroundColor = x.BackgroundColor;
                    uvm.DateLive = x.DateLive;
                    uvm.Duration = x.Duration;
                    uvm.Id = x.Id;
                    uvm.Name = x.Name;
                    uvm.ReleaseYear = x.ReleaseYear;
                    uvm.VzaarVideoId = x.StandardVideoId;
                    uvm.ThumbnailURL = x.ThumbnailURL;
                    if (x.CategoryVideos != null && x.CategoryVideos.Count > 0)
                    {
                        x.CategoryVideos.ToList().ForEach(y =>
                        {
                            uvm.Categories.Add(new UserCategoryModel
                            {
                                Id = y.Category.Id,
                                Name = y.Category.Name
                            });
                        });
                    }
                    videosList.Add(uvm);
                });

                // json result
                var json = new
                {
                    count = 5,
                    data = videosList,
                };

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetLastWatchedVideo")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetLastWatchedVideo()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var lastWatchedVideo = await _Uow._UserVideoHistory.GetAll(x => x.UserId == userId)
                    .OrderByDescending(x => x.WatchDateTime)
                    .Include(x => x.Video)
                    .FirstOrDefaultAsync();

                if (lastWatchedVideo == null)
                {
                    return NotFound();
                }

                var json = new
                {
                    videoId=lastWatchedVideo.VideoId,
                    name=lastWatchedVideo.Video.Name,
                    lastSeekTime=lastWatchedVideo.LastSeekTime.GetValueOrDefault()
                };

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
