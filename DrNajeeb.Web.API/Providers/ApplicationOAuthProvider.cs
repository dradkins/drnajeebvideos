using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using DrNajeeb.Web.API.Models;
using DrNajeeb.Web.API.Hubs;
using Microsoft.AspNet.SignalR;
using System.Data.Entity;

namespace DrNajeeb.Web.API.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (!user.IsActiveUser)
            {
                context.SetError("not_active", user.Id);
                return;
            }

            bool isAccountExpire = false;

            if (user.ExpirationDate != null && DateTime.UtcNow >= user.ExpirationDate.GetValueOrDefault() && user.IsFreeUser.Value)
            {
                isAccountExpire = true;
            }

            var _Uow = new DrNajeeb.Data.Uow(new DrNajeeb.Data.Helpers.RepositoryProvider(new Data.Helpers.RepositoryFactories()));

            if (user.IsFilterByIP)
            {
                var IPAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(IPAddress))
                {
                    IPAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                if (!_Uow._IpAddressFilter.GetAll(x => x.UserId == user.Id).Any(x => x.IpAddress == IPAddress))
                {
                    context.SetError("invalid_ip", user.Id);
                    return;
                }
            }

            /******* Concurrent Views Checking **********/

            var userLogins = await _Uow._LoggedInTracking
                .GetAll(x => x.UserId == user.Id)
                .OrderBy(x => x.DateTimeLoggedIn)
                .ToListAsync();
            string guid = Guid.NewGuid().ToString();
            if (userLogins != null)
            {
                if (userLogins.Count == user.NoOfConcurentViews)
                {
                    var firstUser = userLogins.FirstOrDefault();
                    var hub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    hub.Clients.All.logOut(firstUser.Token);
                    _Uow._LoggedInTracking.Delete(firstUser);
                    await _Uow.CommitAsync();
                }
            }

            _Uow._LoggedInTracking.Add(new EF.LoggedInTracking
            {
                DateTimeLoggedIn = DateTime.Now,
                Token = guid,
                UserId = user.Id
            });
            await _Uow.CommitAsync();

            /****** End Concurrent Views Checking *********/

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user.UserName, user.FullName, user.IsFreeUser.Value, user.SubscriptionId, guid, isAccountExpire);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);

            //var ticketString = context.Ticket.ToString();

            //var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);

            //var _Uow = new DrNajeeb.Data.Uow(new DrNajeeb.Data.Helpers.RepositoryProvider(new Data.Helpers.RepositoryFactories()));

            //_Uow._LoggedInTracking.Add(new EF.LoggedInTracking
            //{
            //    DateTimeLoggedIn = DateTime.UtcNow,
            //    Token = accessToken,
            //    UserId = user.Id
            //});
            //await _Uow.CommitAsync();

            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName, string fullName, bool isFreeUser, int subId, string guid, bool isAccountExpire)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName },
                {"fullName", fullName},
                {"isFreeUser", (isFreeUser)?"True":"False"},
                {"isAccountExpire", (isAccountExpire)?"True":"False"},
                {"showDownloadOption", (subId==1 || subId==3 || subId==6)?"True":"False"},
                {"guid", guid},
            };
            return new AuthenticationProperties(data);
        }
    }
}