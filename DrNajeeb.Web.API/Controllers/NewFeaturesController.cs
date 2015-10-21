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
        [Authorize(Roles="Admin")]
        public async Task<IHttpActionResult> AddFeature(FeatureViewModel model)
        {
            try
            {
                var feature = new NewFeature();
                feature.CreatedBy = User.Identity.GetUserId();
                feature.CreatedOn = DateTime.UtcNow;
                feature.Title = model.Title;
                _Uow._NewFeatures.Add(feature);
                await _Uow.CommitAsync();
                model.Id = feature.Id;
                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("DeleteFeature")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> DeleteFeature(int id)
        {
            try
            {
                await _Uow._NewFeatures.DeleteAsync(id);
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
                return Ok(features);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
