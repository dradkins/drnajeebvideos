using DrNajeeb.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;

namespace DrNajeeb.Web.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie)]
    public class RoleController : BaseController
    {
        public RoleController(IUow uow)
        {
            _Uow = uow;
        }

        [HttpGet]
        [ActionName("GetAll")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var roles = await _Uow._Roles.GetAll().ToListAsync();
                var json = roles.Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name
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
