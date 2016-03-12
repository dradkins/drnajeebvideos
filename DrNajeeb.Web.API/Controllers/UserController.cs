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
using System.IO;
using System.Data.Entity.Core.Objects;
using DrNajeeb.Web.API.Helpers;

namespace DrNajeeb.Web.API.Controllers
{
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
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> GetAll(int page = 1, int itemsPerPage = 20, string sortBy = "CreatedOn", bool reverse = true, string search = null)
        {
            try
            {
                var usersList = new List<UserModel>();

                var users = _Uow._Users.GetAll(x => x.Active == true)
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
                    count = _Uow._Users.Count(),
                    data = usersList,
                };

                //await LogHelpers.SaveLog(_Uow, "Check All  Users", User.Identity.GetUserId());

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [ActionName("AddUser")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> AddUser(UserModel model)
        {
            try
            {
                var subscription=await _Uow._Subscription.GetByIdAsync(model.SubscriptionID.GetValueOrDefault());
                var user = new ApplicationUser()
                {
                    UserName = model.EmailAddress,
                    Email = model.EmailAddress,
                    Active = true,
                    CountryId = (model.CountryID.HasValue) ? model.CountryID.Value : 418,
                    CreatedOn = DateTime.UtcNow,
                    CurrentViews = 0,
                    FullName = model.FullName,
                    IsActiveUser = model.IsActiveUser,
                    IsAllowMobileVideos = true,
                    IsFilterByIP = model.IsFilterByIP,
                    IsParentalControl = false,
                    IsPasswordReset = model.IsPasswordReset,
                    NoOfConcurentViews = model.NoOfConcurrentViews,
                    SubscriptionId = model.SubscriptionID.Value,
                    IsFreeUser = model.IsFreeUser,
                    SubscriptionDate = DateTime.UtcNow,
                    IsInstitutionalAccount = model.IsInstitutionalAccount,
                    ExpirationDate=(subscription==null)?DateTime.UtcNow.AddDays(30):DateTime.UtcNow.AddDays(subscription.TimeDuration.GetValueOrDefault())
                };

                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                result = await UserManager.AddToRolesAsync(user.Id, model.Roles.ToArray());
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }


                var currentUser = await _Uow._Users.GetAll(x => x.Id == user.Id).FirstOrDefaultAsync();

                if (currentUser.IsFilterByIP)
                {
                    if (model.FilteredIPs != null)
                    {
                        foreach (var item in model.FilteredIPs)
                        {
                            currentUser.IpAddressFilters.Add(new EF.IpAddressFilter
                            {
                                CreatedOn = DateTime.UtcNow,
                                IpAddress = item,
                                UserId = currentUser.Id
                            });
                        }
                    }
                }
                currentUser.CreatedBy = User.Identity.GetUserId();

                _Uow._Users.Update(currentUser);
                await _Uow.CommitAsync();

                await LogHelpers.SaveLog(_Uow, "Add User : " + currentUser.UserName, User.Identity.GetUserId());

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetLatestUsers")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
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

                //await LogHelpers.SaveLog(_Uow, "Check Latest Users", User.Identity.GetUserId());

                return Ok(usersList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetUserCount")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> GetUserCount()
        {
            try
            {
                var totalUsers = await _Uow._Users.CountAsync(x => x.Active == true);

                //await LogHelpers.SaveLog(_Uow, "Get Total USers Count", User.Identity.GetUserId());

                return Ok(totalUsers);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("UpdateUser")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> UpdateUser(UserModel model)
        {
            try
            {

                if (!string.IsNullOrEmpty(model.Password))
                {
                    ApplicationUser user = await UserManager.FindByEmailAsync(model.EmailAddress);
                    user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
                    //await UserManager.UpdateSecurityStampAsync(model.EmailAddress);
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    if (result != null && !result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }
                }
                var subscription=await _Uow._Subscription.GetByIdAsync(model.SubscriptionID.GetValueOrDefault());
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
                currentUser.IsFreeUser = model.IsFreeUser;
                currentUser.IsInstitutionalAccount = model.IsInstitutionalAccount;
                currentUser.ExpirationDate = (subscription == null) ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(subscription.TimeDuration.GetValueOrDefault());
                if (currentUser.IsFilterByIP)
                {
                    if (model.FilteredIPs != null)
                    {
                        foreach (var item in model.FilteredIPs)
                        {
                            currentUser.IpAddressFilters.Add(new EF.IpAddressFilter
                            {
                                CreatedOn = DateTime.UtcNow,
                                IpAddress = item,
                                UserId = currentUser.Id
                            });
                        }
                    }
                }
                _Uow._Users.Update(currentUser);
                await _Uow.CommitAsync();

                await LogHelpers.SaveLog(_Uow, "Update User : " + currentUser.UserName, User.Identity.GetUserId());
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("DeleteUser")]
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
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

                await LogHelpers.SaveLog(_Uow, "Delete User : " + user.UserName, User.Identity.GetUserId());
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
            var IPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(IPAddress))
            {
                IPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return Ok(IPAddress);
        }

        [ActionName("UploadProfilePic")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> UploadProfilePic()
        {
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var httpPostedFile = HttpContext.Current.Request.Files["file"];
                using (var reader = new System.IO.BinaryReader(httpPostedFile.InputStream))
                {
                    var imageData = reader.ReadBytes(httpPostedFile.ContentLength);
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    user.ProfilePicture = Convert.ToBase64String(imageData);
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    if (result != null && !result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }
                    return Ok(user.ProfilePicture);
                }

            }
            return Ok();
        }


        [ActionName("GetUserProfilePicture")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetUserProfilePicture()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            return Ok(user.ProfilePicture);
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

        [ActionName("SetFullName")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> SetFullName(SetFullNameViewModel model)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var user = await _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefaultAsync();
                user.FullName = model.FullName;
                _Uow._Users.Update(user);
                await _Uow.CommitAsync();
                return Ok(model.FullName);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("SaveUserToken")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> SaveUserToken(SetFullNameViewModel model)
        {
            var userId = User.Identity.GetUserId();

            _Uow._LoggedInTracking.Add(new EF.LoggedInTracking
            {
                DateTimeLoggedIn = DateTime.UtcNow,
                Token = model.FullName,
                UserId = userId
            });
            await _Uow.CommitAsync();
            return Ok();
        }

        [ActionName("CheckValidity")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> CheckValidity(string id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var user = await _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefaultAsync();

                var loggedInUsers = _Uow._LoggedInTracking.GetAll(x => x.UserId == userId).OrderBy(x => x.DateTimeLoggedIn);

                if (!loggedInUsers.Any(x => x.Token == id))
                {
                    var json = new
                    {
                        Result = false,
                    };
                    return Ok(json);
                }

                if (loggedInUsers.Count() > user.NoOfConcurentViews)
                {
                    var toDelete = await loggedInUsers.FirstAsync();
                    _Uow._LoggedInTracking.Delete(toDelete);
                    await _Uow.CommitAsync();
                    var json = new
                    {
                        Result = true,
                    };
                    return Ok(json);
                }

                var jsonResult = new
                {
                    Result = true,
                };
                return Ok(jsonResult);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [ActionName("GetUserChartData")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IHttpActionResult> GetUserChartData()
        {
            try
            {
                //var total = new List<int>();
                var fromDate = DateTime.UtcNow.AddDays(-7);

                var total = await _Uow._Users.GetAll(x => x.CreatedOn > fromDate)
                    .GroupBy(x => EntityFunctions.TruncateTime(x.CreatedOn))
                    .OrderBy(x => x.Key)
                    .Select(x => new
                    {
                        ActiveUsers = x.Count(y => y.IsActiveUSer),
                        InActiveUsers = x.Count(y => y.IsActiveUSer == false),
                        FreeUsers = x.Count(y => y.IsFreeUser.Value),
                        Day = (DateTime)x.Key
                    })
                    .ToListAsync();

                //var users = _Uow._Users.GetAll(x => x.CreatedOn > fromDate).GroupBy(x => EntityFunctions.TruncateTime(x.CreatedOn));

                //foreach (var item in users)
                //{
                //    total.Add(item.Count());
                //}

                //await LogHelpers.SaveLog(_Uow, "View User Chart Data", User.Identity.GetUserId());

                return Ok(total);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("CheckInstitutionalUser")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> CheckInstitutionalUser()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                bool isInstitutionalAccount = await _Uow._Users.GetAll(x => x.Id == userId).Select(x => x.IsInstitutionalAccount).FirstOrDefaultAsync() ?? false;
                return Ok(isInstitutionalAccount);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("IsUserLoggedIn")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> IsUserLoggedIn(string id)
        {
            try
            {
                bool value = _Uow._LoggedInTracking.GetAll(x => x.Token == id).Any();
                return Ok(value);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [ActionName("GetLastLogintime")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetLastLogintime()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var lastLoginTime = await _Uow._LoggedInTracking.GetAll(x => x.UserId == userId).OrderByDescending(x => x.DateTimeLoggedIn).FirstOrDefaultAsync();
                if (lastLoginTime == null)
                {
                    return Ok();
                }
                else
                {
                    return Ok(lastLoginTime.DateTimeLoggedIn);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetManagers")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> GetManagers()
        {
            try
            {
                var managers = await _Uow._Roles.GetAll(x => x.Name == "Manager")
                    .Include(x => x.AspNetUsers)
                    .Select(x => x.AspNetUsers)
                    .ToListAsync();

                var usersList = new List<UserModel>();

                foreach (var parent in managers)
                {
                    foreach (var item in parent)
                    {
                        if (item.Active)
                        {
                            var usermodel = new UserModel();
                            usermodel.EmailAddress = item.Email;
                            usermodel.FullName = item.FullName;
                            usermodel.Id = item.Id;
                            usersList.Add(usermodel);
                        }
                    }
                }

                await LogHelpers.SaveLog(_Uow, "Get All Managers", User.Identity.GetUserId());

                return Ok(usersList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }

    public class FileActionResult : IHttpActionResult
    {
        public FileActionResult(int fileId)
        {
            this.FileId = fileId;
        }

        public int FileId { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StreamContent(File.OpenRead(@"<base path>" + FileId));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");

            // NOTE: Here I am just setting the result on the Task and not really doing any async stuff. 
            // But let's say you do stuff like contacting a File hosting service to get the file, then you would do 'async' stuff here.

            return Task.FromResult(response);
        }
    }
}
