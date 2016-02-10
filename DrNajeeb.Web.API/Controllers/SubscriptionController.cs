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
using System.Data.Entity;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity;
using DrNajeeb.Web.API.Helpers;

namespace DrNajeeb.Web.API.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie)]
    public class SubscriptionController : BaseController
    {
        public SubscriptionController(IUow uow)
        {
            _Uow = uow;
        }

        public async Task<IHttpActionResult> GetAll(int page = 1, int itemsPerPage = 20, string sortBy = "Name", bool reverse = false, string search = null)
        {
            try
            {
                var subscriptionList = new List<SubscriptionModel>();
                var subscriptions = _Uow._Subscription.GetAll(x => x.Active == true);

                // searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    subscriptions = subscriptions.Where(x =>
                        x.Description.ToLower().Contains(search) ||
                        x.GatewayId.ToLower().Contains(search) ||
                        x.GatewayURL.ToLower().Contains(search) ||
                        x.Name.ToLower().Contains(search));
                }

                // sorting (done with the System.Linq.Dynamic library available on NuGet)
                subscriptions = subscriptions.OrderBy(sortBy + (reverse ? " descending" : ""));

                // paging
                var subscriptionsPaged = await subscriptions.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();

                //Converting to modal object
                subscriptionsPaged.ForEach(x =>
                {
                    subscriptionList.Add(new SubscriptionModel
                    {
                        Description = x.Description,
                        EndDate = x.EndDate,
                        GatewayId = x.GatewayId,
                        Id = x.Id,
                        IsActiveSubscription = x.IsActiveSubscription,
                        Name = x.Name,
                        Price = x.Price,
                        StartDate = x.StartDate,
                        TimeDurationInDays = x.TimeDuration
                    });
                });

                // json result
                var json = new
                {
                    count = _Uow._Subscription.Count(),
                    data = subscriptionList
                };

                //await LogHelpers.SaveLog(_Uow, "View All Subscription", User.Identity.GetUserId());

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [ActionName("AddSubscription")]
        [HttpPost]
        public async Task<IHttpActionResult> AddSubscription(SubscriptionModel model)
        {
            try
            {
                var subscription = new Subscription();
                subscription.Description = model.Description;
                subscription.EndDate = model.EndDate;
                subscription.GatewayId = model.GatewayId;
                subscription.IsActiveSubscription = model.IsActiveSubscription ?? true;
                subscription.Name = model.Name;
                subscription.Price = model.Price ?? 0;
                subscription.StartDate = model.StartDate;
                subscription.TimeDuration = model.TimeDurationInDays;
                subscription.Active = true;
                subscription.CreatedOn = DateTime.UtcNow;
                subscription.CreatedBy = User.Identity.GetUserId();

                await LogHelpers.SaveLog(_Uow, "Create Subscription " + subscription.Name, User.Identity.GetUserId());

                _Uow._Subscription.Add(subscription);
                await _Uow.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("DeleteSubscription")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteSubscription(int id)
        {
            try
            {
                var subscription = await _Uow._Subscription.GetByIdAsync(id);
                if (subscription == null)
                {
                    return NotFound();
                }

                subscription.Active = false;
                subscription.UpdatedOn = DateTime.Now;
                subscription.UpdatedBy = User.Identity.GetUserId();
                _Uow._Subscription.Update(subscription);

                await LogHelpers.SaveLog(_Uow, "Delete Subscription " + subscription.Name, User.Identity.GetUserId());

                await _Uow.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("UpdateSubscription")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateSubscription(SubscriptionModel model)
        {
            try
            {
                var subscription = await _Uow._Subscription.GetByIdAsync(model.Id);
                subscription.Description = model.Description;
                subscription.EndDate = model.EndDate;
                subscription.GatewayId = model.GatewayId;
                subscription.IsActiveSubscription = model.IsActiveSubscription ?? true;
                subscription.Name = model.Name;
                subscription.Price = model.Price ?? 0;
                subscription.StartDate = model.StartDate;
                subscription.TimeDuration = model.TimeDurationInDays;
                subscription.UpdatedBy = User.Identity.GetUserId();

                _Uow._Subscription.Update(subscription);
                await _Uow.CommitAsync();

                await LogHelpers.SaveLog(_Uow, "Update Subscription " + subscription.Name, User.Identity.GetUserId());

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetSingle")]
        [HttpPost]
        public async Task<IHttpActionResult> GetSingle(int Id)
        {
            try
            {
                var subscription = await _Uow._Subscription.GetByIdAsync(Id);
                if (subscription == null)
                {
                    return NotFound();
                }

                var model = await _Uow._Subscription.GetByIdAsync(Id);
                model.Description = subscription.Description;
                model.EndDate = subscription.EndDate;
                model.GatewayId = subscription.GatewayId;
                model.IsActiveSubscription = subscription.IsActiveSubscription;
                model.Name = subscription.Name;
                model.Price = subscription.Price;
                model.StartDate = subscription.StartDate;
                model.TimeDuration = subscription.TimeDuration;

                //await LogHelpers.SaveLog(_Uow, "View Single Subscription "+subscription.Name, User.Identity.GetUserId());

                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }




        [ActionName("GetSubscriptionsForUser")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetSubscriptionsForUser()
        {
            try
            {
                var subscriptions = await _Uow._Subscription
                    .GetAll(x => x.Active == true && x.IsActiveSubscription == true)
                    .Select(x => new
                    {
                        Name = x.Name,
                        ProductId = x.GatewayId,
                        Id = x.Id,
                        Price = x.Price
                    })
                    .ToListAsync();
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [ActionName("GetUserSubscriptionDetails")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetUserSubscriptionDetails()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var userSubscription = await _Uow._Users
                    .GetAll(x => x.Id == userId)
                    .Include(x => x.Subscription)
                    .Select(x => new
                    {
                        Name = x.Subscription.Name,
                        Description=x.Subscription.Description
                    })
                .FirstOrDefaultAsync();
                return Ok(userSubscription);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
