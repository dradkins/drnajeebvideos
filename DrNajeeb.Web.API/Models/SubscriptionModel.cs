using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrNajeeb.Web.API.Models
{
    public class SubscriptionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<double> Price { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<bool> IsActiveSubscription { get; set; }
        public Nullable<int> TimeDurationInDays { get; set; }
        public string GatewayId { get; set; }
    }

    class SubscriptionRevenueViewModel
    {
        public string SubscriptionName { get; set; }
        public int TotalUsers { get; set; }
        public int SubscriptionId { get; set; }
        public double Amount { get; set; }
    }
}