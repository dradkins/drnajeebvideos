using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using DrNajeeb.Web.API.Models;
using DrNajeeb.Web.API.Providers;
using DrNajeeb.Web.API.Results;
using DrNajeeb.Data;
using DrNajeeb.Data.Helpers;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using System.Net;
using DrNajeeb.Web.API.Hubs;
using System.Linq;
using DrNajeeb.Contract;
using DrNajeeb.Web.API.Helpers;

namespace DrNajeeb.Web.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : BaseController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController(IUow uow)
        {
            _Uow = uow;
        }

        public AccountController(ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

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

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalBearer);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalBearer);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                bool isAccountExpire = false;

                if (user.ExpirationDate != null && DateTime.UtcNow >= user.ExpirationDate.GetValueOrDefault() && user.IsFreeUser.Value)
                {
                    isAccountExpire = true;
                }

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName, user.FullName, user.IsFreeUser.Value, user.SubscriptionId, Guid.NewGuid().ToString(), isAccountExpire);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("RegisterAllUsers")]
        public async Task<IHttpActionResult> RegisterAllUsers(int id)
        {
            var identityResult = new List<IdentityResult>();

            string physicalPath = @"~/Content/drnajeebdata1.txt";
            String strNewPath = HttpContext.Current.Server.MapPath(physicalPath);
            try
            {
                var emails = System.IO.File.ReadLines(strNewPath).OrderBy(x => x).Skip(100 * id).Take(100);
                foreach (var email1 in emails)
                {
                    var email = System.Text.RegularExpressions.Regex.Replace(email1, @"\s+", "");
                    var user = new ApplicationUser()
                    {
                        UserName = email,
                        Email = email,
                        FullName = "",
                        CountryId = 418,
                        CreatedOn = DateTime.UtcNow,
                        CurrentViews = 0,
                        IsActiveUser = true,
                        IsAllowMobileVideos = true,
                        IsFilterByIP = false,
                        IsParentalControl = false,
                        IsPasswordReset = false,
                        NoOfConcurentViews = 1,
                        SubscriptionDate = DateTime.UtcNow,
                        SubscriptionId = 1,
                        ExpirationDate = null,
                        Active = true
                    };

                    IdentityResult result = await UserManager.CreateAsync(user, email);

                    if (!result.Succeeded)
                    {
                        identityResult.Add(result);
                        //return GetErrorResult(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            return Ok();
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                CountryId = model.CountryId,
                CreatedOn = DateTime.UtcNow,
                CurrentViews = 0,
                IsActiveUser = model.IsFreeUser,
                IsAllowMobileVideos = true,
                IsFilterByIP = false,
                IsParentalControl = false,
                IsPasswordReset = false,
                NoOfConcurentViews = 1,
                SubscriptionDate = DateTime.UtcNow,
                SubscriptionId = model.SubscriptionId,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                Active = true,
                IsFreeUser = true,
                IsInstitutionalAccount = false

            };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok(user.Id);
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        // POST api/Account/RegisterExternalToken
        [OverrideAuthentication]
        [AllowAnonymous]
        [Route("RegisterExternalToken")]
        public async Task<IHttpActionResult> RegisterExternalToken(RegisterExternalTokenBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExternalLoginData externalLogin = await ExternalLoginData.FromToken(model.Provider, model.Token);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != model.Provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return InternalServerError();
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;
            ClaimsIdentity identity = null;
            IdentityResult result;

            if (hasRegistered)
            {
                identity = await UserManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                identity.AddClaims(claims);
                Authentication.SignIn(identity);
                if (user.IsActiveUser)
                {
                    AuthenticationTicket ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
                    var currentUtc = new Microsoft.Owin.Infrastructure.SystemClock().UtcNow;
                    ticket.Properties.IssuedUtc = currentUtc;
                    ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromDays(365));
                    var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);
                    Request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);



                    // Create the response building a JSON object that mimics exactly the one issued by the default /Token endpoint
                    JObject token = new JObject(
                        new JProperty("userName", user.UserName),
                        new JProperty("isFreeUser", user.IsFreeUser),
                        new JProperty("id", user.Id),
                        new JProperty("fullName", user.FullName),
                        new JProperty("access_token", accessToken),
                        new JProperty("token_type", "bearer"),
                        new JProperty("showDownloadOption", (user.SubscriptionId == 1)),
                        new JProperty("expires_in", TimeSpan.FromDays(365).TotalSeconds.ToString()),
                        new JProperty(".issued", currentUtc.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'")),
                        new JProperty(".expires", currentUtc.Add(TimeSpan.FromDays(365)).ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"))
                    );

                    //_Uow._LoggedInTracking.Add(new EF.LoggedInTracking
                    //{
                    //    DateTimeLoggedIn = DateTime.UtcNow,
                    //    Token = accessToken,
                    //    UserId = user.Id
                    //});
                    //await _Uow.CommitAsync();

                    return Ok(user.Id);
                }
            }
            else
            {
                user = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    CountryId = model.CountryId,
                    CreatedOn = DateTime.UtcNow,
                    CurrentViews = 0,
                    IsActiveUser = true,
                    IsAllowMobileVideos = true,
                    IsFilterByIP = false,
                    IsParentalControl = false,
                    IsPasswordReset = false,
                    NoOfConcurentViews = 1,
                    SubscriptionDate = DateTime.UtcNow,
                    SubscriptionId = 2,
                    ExpirationDate = DateTime.UtcNow.AddDays(30),
                    Active = true,
                    IsFreeUser = true,
                    IsInstitutionalAccount=false,
                };

                result = await UserManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                var info = new ExternalLoginInfo()
                {
                    DefaultUserName = model.Email,
                    Login = new UserLoginInfo(model.Provider, externalLogin.ProviderKey)
                };

                result = await UserManager.AddLoginAsync(user.Id, info.Login);
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                identity = await UserManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                identity.AddClaims(claims);
                //Authentication.SignIn(identity);
            }

            //AuthenticationTicket ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            //var currentUtc = new Microsoft.Owin.Infrastructure.SystemClock().UtcNow;
            //ticket.Properties.IssuedUtc = currentUtc;
            //ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromDays(365));
            //var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);
            //Request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);



            //// Create the response building a JSON object that mimics exactly the one issued by the default /Token endpoint
            //JObject token = new JObject(
            //    new JProperty("userName", user.UserName),
            //    new JProperty("id", user.Id),
            //    new JProperty("fullName", user.FullName),
            //    new JProperty("access_token", accessToken),
            //    new JProperty("token_type", "bearer"),
            //    new JProperty("expires_in", TimeSpan.FromDays(365).TotalSeconds.ToString()),
            //    new JProperty(".issued", currentUtc.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'")),
            //    new JProperty(".expires", currentUtc.Add(TimeSpan.FromDays(365)).ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"))
            //);
            return Ok(user.Id);
        }


        [AllowAnonymous]
        [Route("ExternalSignin")]
        public async Task<IHttpActionResult> ExternalSignin(SigninExternalTokenBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExternalLoginData externalLogin = await ExternalLoginData.FromToken(model.Provider, model.Token);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != model.Provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return InternalServerError();
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;
            ClaimsIdentity identity = null;

            if (hasRegistered)
            {
                if (!user.IsActiveUser)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Forbidden);

                    response.ReasonPhrase = user.Id;
                    return ResponseMessage(response);
                }

                //var hub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                //hub.Clients.All.logout(user.Email);

                //var loggedInUsers = NotificationHub._connections.GetConnections(user.Email);
                //if (loggedInUsers.Count() > 0)
                //{
                //    var hub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                //    foreach (var connectionId in loggedInUsers)
                //    {
                //        hub.Clients.Client(connectionId).logout();
                //    }
                //}

                identity = await UserManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                identity.AddClaims(claims);
                Authentication.SignIn(identity);
            }
            else
            {
                return NotFound();
            }

            AuthenticationTicket ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            var currentUtc = new Microsoft.Owin.Infrastructure.SystemClock().UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromDays(365));
            var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);
            Request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);



            // Create the response building a JSON object that mimics exactly the one issued by the default /Token endpoint
            JObject token = new JObject(
                new JProperty("userName", user.UserName),
                new JProperty("isFreeUser", user.IsFreeUser),
                new JProperty("id", user.Id),
                new JProperty("fullName", user.FullName),
                new JProperty("access_token", accessToken),
                new JProperty("token_type", "bearer"),
                new JProperty("showDownloadOption", (user.SubscriptionId == 1)),
                new JProperty("expires_in", TimeSpan.FromDays(365).TotalSeconds.ToString()),
                new JProperty(".issued", currentUtc.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'")),
                new JProperty(".expires", currentUtc.Add(TimeSpan.FromDays(365)).ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"))
            );

            //_Uow._LoggedInTracking.Add(new EF.LoggedInTracking
            //{
            //    DateTimeLoggedIn = DateTime.UtcNow,
            //    Token = accessToken,
            //    UserId = user.Id
            //});
            //await _Uow.CommitAsync();

            return Ok(token);
        }

        [Authorize(Roles = "Admin,Manager")]
        [Route("UpdateUserName")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateUserName(ChangeUserNameBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await UserManager.FindByEmailAsync(model.OldEmail);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.NewEmail;
            user.UserName = model.NewEmail;

            IdentityResult result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            await LogHelpers.SaveLog(_Uow, "Update user name from " + model.OldEmail + " to " + model.NewEmail, User.Identity.GetUserId());

            return Ok();
        }


        /****** Change Pasword ********/



        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please enter valid email address");
                return BadRequest(ModelState);
            }

            ApplicationUser user = await UserManager.FindByNameAsync(model.Email);

            //for security purpose don't show the user id
            if (user == null)
            {
                return Ok();
            }

            var pwd = RandomUtil.GetRandomNumberPassword(8);
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, code, pwd);
            if (result.Succeeded)
            {
                await UserManager.SendEmailAsync(user.Id, "Password Reset", "Your password reset successfully, Please change the password when you login. New password is " + pwd);
                return Ok();
            }
            return GetErrorResult(result);

            //string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            //var callbackUrl = string.Format("http://members.drnajeeblectures.com/#/reset-password/{0}/{1}", user.Id, HttpUtility.UrlEncode(code).Replace("%2f", "%252F"));
            //await UserManager.SendEmailAsync(user.Id, "Reset your password", callbackUrl);

            //return Ok();
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[Route("ResetPassword")]
        //public async Task<IHttpActionResult> ResetPassword(ResetPasswordBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var user = await UserManager.FindByIdAsync(model.UserId);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return Ok();
        //    }
        //    return GetErrorResult(result);
        //}


        /******* Change Password **********/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UserManager.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
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
                        ModelState.AddModelError("", error);
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

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string ExternalAccessToken { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
                    ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken"),
                };
            }

            public static async Task<ExternalLoginData> FromToken(string provider, string accessToken)
            {

                string verifyTokenEndPoint = "", verifyAppEndpoint = "";

                if (provider == "Facebook")
                {
                    verifyTokenEndPoint = string.Format("https://graph.facebook.com/me?access_token={0}", accessToken);
                    verifyAppEndpoint = string.Format("https://graph.facebook.com/app?access_token={0}", accessToken);
                }
                else if (provider == "Google")
                {
                    // not implemented yet
                    //return null;
                    verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
                }
                else
                {
                    return null;
                }

                HttpClient client = new HttpClient();
                Uri uri = new Uri(verifyTokenEndPoint);
                HttpResponseMessage response = await client.GetAsync(uri);
                ClaimsIdentity identity = null;
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    dynamic iObj = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                    uri = new Uri(verifyAppEndpoint);
                    response = await client.GetAsync(uri);
                    content = await response.Content.ReadAsStringAsync();
                    dynamic appObj = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                    identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

                    if (provider == "Facebook")
                    {
                        //if (appObj["id"] != Startup.facebookAuthOptions.AppId)
                        //{
                        //    return null;
                        //}

                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, iObj["id"].ToString(), ClaimValueTypes.String, "Facebook", "Facebook"));

                    }
                    else if (provider == "Google")
                    {
                        //not implemented yet
                    }

                }

                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer) || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }
                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
