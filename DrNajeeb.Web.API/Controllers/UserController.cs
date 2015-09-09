using DrNajeeb.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using DrNajeeb.Web.API.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DrNajeeb.Web.API.Controllers
{
    [Authorize]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie)]
    public class UserController : BaseController
    {

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public UserController(IUow uow)
        {
            _Uow = uow;
        }

        [ActionName("GetAll")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll(int page = 1, int itemsPerPage = 20, string sortBy = "FullName", bool reverse = false, string search = null)
        {
            try
            {
                var usersList = new List<UserModel>();

                var users = _Uow._Users.GetAll(x => x.Active == true)
                    .Include(x => x.Country)
                    .Include(x => x.Subscription)
                    .Include(x => x.AspNetRoles);

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
                    usersList.Add(usermodel);
                }

                // json result
                var json = new
                {
                    count = _Uow._Users.Count(),
                    data = usersList,
                };

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [ActionName("AddUser")]
        [HttpPost]
        public async Task<IHttpActionResult> AddUser(UserModel model)
        {
            try
            {
                var user = new ApplicationUser() { UserName = model.EmailAddress, Email = model.EmailAddress };

                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
                var currentUser = await _Uow._Users.GetAll(x => x.Id == user.Id).FirstOrDefaultAsync();
                currentUser.Active = true;
                currentUser.CountryId = model.CountryID;
                currentUser.CreatedOn = DateTime.UtcNow;
                currentUser.FullName = model.FullName;
                currentUser.IsActiveUSer = model.IsActiveUser;
                currentUser.IsAllowMobileVideos = true;
                currentUser.IsFilterByIP = model.IsFilterByIP;
                currentUser.IsParentalControl = false;
                currentUser.IsPasswordReset = model.IsPasswordReset;
                currentUser.NoOfConcurentViews = model.NoOfConcurrentViews;
                currentUser.SubscriptionId = model.SubscriptionID;

                if (currentUser.IsFilterByIP)
                {
                    if (model.FilteredIPs != null)
                    {
                        foreach (var item in model.FilteredIPs)
                        {
                            currentUser.IpAddressFilters.Add(new EF.IpAddressFilter
                            {
                                CreatedOn = DateTime.UtcNow,
                                IpAddress = item
                            });
                        }
                    }
                }
                currentUser.CreatedBy = User.Identity.GetUserId();

                _Uow._Users.Update(currentUser);
                await _Uow.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetLatestUsers")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLatestUsers()
        {
            try
            {
                var usersList = new List<UserModel>();

                var users = await _Uow._Users.GetAll(x => x.Active == true)
                    .Include(x => x.Country)
                    .Include(x => x.Subscription)
                    .Include(x => x.AspNetRoles)
                    .OrderByDescending(x => x.CreatedOn)
                    .Take(10)
                    .ToListAsync();

                foreach (var item in users)
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
                    usersList.Add(usermodel);
                }

                return Ok(usersList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetUserCount")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserCount()
        {
            try
            {
                var totalUsers = await _Uow._Users.CountAsync(x => x.Active == true);
                return Ok(totalUsers);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("UpdateUser")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateUser(UserModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.Password))
                {
                    ApplicationUser user = await UserManager.FindByIdAsync(model.EmailAddress);
                    user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
                    await UserManager.UpdateSecurityStampAsync(model.EmailAddress);
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    if (result != null && !result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }
                }
                var currentUser = await _Uow._Users.GetAll(x => x.Email == model.EmailAddress).FirstOrDefaultAsync();
                currentUser.CountryId = model.CountryID;
                currentUser.UpdatedOn = DateTime.UtcNow;
                currentUser.FullName = model.FullName;
                currentUser.IsActiveUSer = model.IsActiveUser;
                currentUser.IsAllowMobileVideos = true;
                currentUser.IsFilterByIP = model.IsFilterByIP;
                currentUser.IsParentalControl = false;
                currentUser.IsPasswordReset = model.IsPasswordReset;
                currentUser.NoOfConcurentViews = model.NoOfConcurrentViews;
                currentUser.SubscriptionId = model.SubscriptionID;
                currentUser.UpdatedBy = User.Identity.GetUserId();
                if (currentUser.IsFilterByIP)
                {
                    if (model.FilteredIPs != null)
                    {
                        foreach (var item in model.FilteredIPs)
                        {
                            currentUser.IpAddressFilters.Add(new EF.IpAddressFilter
                            {
                                CreatedOn = DateTime.UtcNow,
                                IpAddress = item
                            });
                        }
                    }
                }
                _Uow._Users.Update(currentUser);
                await _Uow.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("DeleteUser")]
        [HttpPost]
        public async Task<IHttpActionResult> DeleteUser([FromBody]string userId)
        {
            try
            {
                var user = await _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound();
                }
                user.Active = false;
                user.UpdatedBy = User.Identity.GetUserId();
                user.UpdatedOn = DateTime.UtcNow;
                _Uow._Users.Update(user);
                await _Uow.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetIPAddress")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetIPAddress()
        {
            string userIpAddress = HttpContext.Current.Request.UserHostAddress;
            return Ok(userIpAddress);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("ModelError", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
