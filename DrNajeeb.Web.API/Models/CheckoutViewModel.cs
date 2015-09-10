using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DrNajeeb.Web.API.Models
{
    public class CheckoutViewModel
    {
        public string Country { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
    }

    public class ConfirmCheckoutViewModel
    {
        public string Message { get; set; }
        public string Result { get; set; }
    }
}