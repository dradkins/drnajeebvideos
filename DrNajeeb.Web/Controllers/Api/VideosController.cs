using DrNajeeb.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Linq.Dynamic;
using DrNajeeb.Web.Models;
using System.Threading.Tasks;

namespace DrNajeeb.Web.Controllers.Api
{
    public class VideosController : BaseController
    {
        public VideosController(IUow uow)
        {
            _Uow = uow;
        }

        [ActionName("GetAll")]
        [HttpGet]
        public IHttpActionResult GetAll(int page = 1, int itemsPerPage = 20, string sortBy = "Name", bool reverse = false, string search = null)
        {
            try
            {
                var videos = _Uow._Videos.GetAll(x => x.Active == true);

                // searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    videos = videos.Where(x =>
                        x.Name.ToLower().Contains(search) ||
                        x.Description.ToLower().Contains(search));
                }

                // sorting (done with the System.Linq.Dynamic library available on NuGet)
                videos = videos.OrderBy(sortBy + (reverse ? " descending" : ""));

                // paging
                var videosPaged = videos.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);

                // json result
                var json = new
                {
                    count = _Uow._Videos.Count(),
                    data = videosPaged,
                };

                return Ok(json);
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

                //todo : add useid in createdby

                _Uow._Videos.Add(video);
                await _Uow.CommitAsync();
                return Ok();
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
        public async Task<IHttpActionResult> UpdateSubscription(VideoModel model)
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
    }
}
