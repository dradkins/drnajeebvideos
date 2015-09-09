using DrNajeeb.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DrNajeeb.Web.API.Controllers
{
    [Authorize]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie)]
    public class CountryController : BaseController
    {
        public CountryController(IUow uow)
        {
            _Uow = uow;
        }

        [AllowAnonymous]
        [HttpGet]
        [ActionName("GetAll")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var countries = await _Uow._Country.GetAll().ToListAsync();
                var json = countries.Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name, 
                    ISO2Name=x.ISO2Name
                });
                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
