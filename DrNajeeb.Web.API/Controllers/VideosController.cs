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

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [ActionName("GetByCategoryForSorting")]
        [HttpGet]
        public IHttpActionResult GetByCategoryForSorting(int id)
        {
            try
            {
                var videosList = new List<VideoModel>();

                var videos = _Uow._CategoryVideos.GetAll(x => x.CategoryId == id)
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

                return Ok(videosList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [ActionName("UpdateOrder")]
        [HttpPost]
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
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("AddVideo")]
        [HttpPost]
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
                    Id=video.Id,
                    Name=video.Name
                };
                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("DeleteVideo")]
        [HttpDelete]
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

                //todo : add useid in updatedby

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

                //todo : add useid in updatedby

                _Uow._Videos.Update(video);
                await _Uow.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetSingle")]
        [HttpGet]
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
                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetVideosCount")]
        public async Task<IHttpActionResult> GetVideosCount()
        {
            try
            {
                var videosCount = await _Uow._Videos.CountAsync(x => x.Active == true);
                return Ok(videosCount);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [ActionName("GetByCategory")]
        [HttpGet]
        public async Task<IHttpActionResult> GetByCategory(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var videosList = new List<UserVideoModel>();
                var videos = await _Uow._CategoryVideos
                    .GetAll(x => x.CategoryId == id)
                    .Include(x => x.Video)
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => x.Video)
                    .Include(x => x.UserFavoriteVideos)
                    .ToListAsync();

                videos.ForEach(x => videosList.Add(new UserVideoModel
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
        public async Task<IHttpActionResult> GetUserVideo(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();

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
                videosModel.BackgroundColor = video.BackgroundColor;
                videosModel.DateLive = video.DateLive;
                videosModel.Duration = video.Duration;
                videosModel.Id = video.Id;
                videosModel.Name = video.Name;
                videosModel.ReleaseYear = video.ReleaseYear;
                videosModel.VzaarVideoId = video.StandardVideoId;
                videosModel.ThumbnailURL = video.ThumbnailURL;
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


                //add video to user history
                var userHistory = new UserVideoHistory();
                userHistory.UserId = userId;
                userHistory.VideoId = id;
                userHistory.WatchDateTime = DateTime.UtcNow;
                _Uow._UserVideoHistory.Add(userHistory);
                await _Uow.CommitAsync();

                return Ok(videosModel);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("getUserNewVideo")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserNewVideo(int page = 1, int itemsPerPage = 20, string search = null)
        {
            try
            {
                var userId = User.Identity.GetUserId();
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

                var totalVideos = await videos.CountAsync();
                videos = videos.OrderBy("DateLive");

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
        public async Task<IHttpActionResult> DeleteUserVideoToFavorite(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
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
        public async Task<IHttpActionResult> GetUserFavoriteVideos(int page = 1, int itemsPerPage = 20, string search = null)
        {
            try
            {
                var userId = User.Identity.GetUserId();
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

                var totalVideos = await videos.CountAsync();
                videos = videos.OrderBy("DateLive");

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

        [ActionName("GetUserVideosHistory")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserVideosHistory(int page = 1, int itemsPerPage = 20, string search = null)
        {
            try
            {
                var userId = User.Identity.GetUserId();
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

                var totalVideos = await videos.CountAsync();
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
                var videosList = new List<UserVideoModel>();
                var videos = await _Uow._CategoryVideos
                    .GetAll(x => x.CategoryId == id)
                    .Include(x => x.Video)
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => x.Video)
                    .ToListAsync();

                videos.ForEach(x => videosList.Add(new UserVideoModel
                {
                    Name = x.Name,
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
                if (!string.IsNullOrEmpty(id))
                {
                    var videos = await _Uow._Videos.GetAll(x => x.Name.Contains(id)).Select(x => x.Name).ToListAsync();
                    return Ok(videos);
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
        public async Task<IHttpActionResult> VideoNotifications()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var user = await _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefaultAsync();
                var videosList = new List<UserVideoModel>();

                var videos = _Uow._Videos.GetAll(x => x.CreatedOn > user.CreatedOn)
                    .Include(x => x.UserVideoHistories.Where(y => y.UserId == userId))
                    .Where(x => !x.UserVideoHistories.Any(y => y.Id == x.Id));

                foreach (var item in videos)
                {
                    videosList.Add(new UserVideoModel
                    {
                        Id=item.Id,
                        Name=item.Name
                    });
                }
                return Ok(videosList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
