using DrNajeeb.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using DrNajeeb.Web.API.Models;

namespace DrNajeeb.Web.API.Controllers
{
    public class HomeController : Controller
    {
        IUow _UOW;

        public HomeController(IUow uow)
        {
            _UOW = uow;
        }


        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }

        public ActionResult Admin()
        {
            ViewBag.Title = "Admin Panel";
            return View();
        }

        public async Task<ActionResult> Checkout(string id)
        {
            var user = await _UOW._Users.GetAll(x => x.Id == id)
                        .Include(x => x.Country)
                        .FirstOrDefaultAsync();

            if (user == null)
            {
                return HttpNotFound("user not found");
            }

            CheckoutViewModel model = new CheckoutViewModel();
            model.EmailAddress = user.Email;
            model.FullName = user.FullName;
            model.Country = user.Country.Name;
            ViewBag.Title = "Checkout";
            return View(model);
        }

        public async Task<ActionResult> CompleteCheckout(
            string order_number = "",
            string total = "",
            string credit_card_processed = "",
            string key = "",
            string email = "")
        {
            var secretWord = System.Configuration.ConfigurationManager.AppSettings["2CheckoutSecretWord"].ToString();
            var sellerId = System.Configuration.ConfigurationManager.AppSettings["2CheckoutSellerId"].ToString();

            string toHashed = secretWord + sellerId + order_number + total;

            var verifier = DrNajeeb.Web.API.Helpers.HashHelper.CreateMD5(toHashed);
            if (verifier == key)
            {
                var user = await _UOW._Users.GetAll(x => x.Email == email).FirstOrDefaultAsync();
                if (user != null)
                {
                    user.IsActiveUSer = true;
                    user.IsFreeUser = false;
                    _UOW._Users.Update(user);
                    await _UOW.CommitAsync();

                    var model=new ConfirmCheckoutViewModel()
                    {
                        Result="SUCCESS",
                        Message="congratulations! Your account is activated successfully",
                    };

                    return View(model);
                }
                else
                {
                    var model=new ConfirmCheckoutViewModel()
                    {
                        Result="NOT FOUND",
                        Message="Please contact the admin for further assistant",
                    };

                    return View(model);
                }
            }
            else
            {
                var model=new ConfirmCheckoutViewModel()
                    {
                        Result="ERROR",
                        Message="Unable to verify your payment",
                    };

                    return View(model);
            }
        }
    }
}
