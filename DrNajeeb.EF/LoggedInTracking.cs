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
    
    public partial class LoggedInTracking
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public System.DateTime DateTimeLoggedIn { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual AspNetUser AspNetUser2 { get; set; }
        public virtual AspNetUser AspNetUser3 { get; set; }
    }
}