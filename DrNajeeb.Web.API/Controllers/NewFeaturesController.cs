using DrNajeeb.Contract;
using DrNajeeb.EF;
using DrNajeeb.Web.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using DrNajeeb.Web.API.Helpers;

namespace DrNajeeb.Web.API.Controllers
{
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie)]
    public class NewFeaturesController :BaseController
    {

        public NewFeaturesController(IUow uow)
        {
            _Uow = uow;
        }

        [ActionName("AddFeature")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> AddFeature(FeatureViewModel model)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var feature = new NewFeature();
                feature.CreatedBy = userId;
                feature.CreatedOn = DateTime.UtcNow;
                feature.Title = model.Title;
                _Uow._NewFeatures.Add(feature);
                await _Uow.CommitAsync();
                model.Id = feature.Id;

                await LogHelpers.SaveLog(_Uow, "Add Feature " + model.Title, userId);

                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("DeleteFeature")]
        [HttpDelete]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> DeleteFeature(int id)
        {
            try
            {
                var feature = await _Uow._NewFeatures.GetByIdAsync(id);
                if (feature == null)
                {
                    return NotFound();
                }
                _Uow._NewFeatures.Delete(feature);

                await LogHelpers.SaveLog(_Uow, "Delete Feature "+feature.Title, User.Identity.GetUserId());
                await _Uow.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetAll")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var features = new List<FeatureViewModel>();
                var dbFeatures = await _Uow._NewFeatures.GetAll().ToListAsync();
                dbFeatures.ForEach(x =>
                {
                    features.Add(new FeatureViewModel
                    {
                        Id=x.Id,
                        Title=x.Title
                    });
                });

                await LogHelpers.SaveLog(_Uow, "View All Features", User.Identity.GetUserId());

                return Ok(features);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
