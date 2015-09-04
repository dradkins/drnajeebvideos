using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrNajeeb.Web.API.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public Nullable<bool> IsShowOnFrontPage { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string SeoName { get; set; }
        public string CategoryURL { get; set; }
        public int DisplayOrder { get; set; }
    }
}