//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DrNajeeb.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class CategoriesInSubscription
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int SubscriptionId { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Category Category1 { get; set; }
        public virtual Subscription Subscription { get; set; }
        public virtual Subscription Subscription1 { get; set; }
    }
}
