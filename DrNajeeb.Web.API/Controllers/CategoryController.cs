using DrNajeeb.Contract;
using DrNajeeb.Web.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq.Dynamic;
using System.Data.Entity;
using DrNajeeb.EF;

namespace DrNajeeb.Web.API.Controllers
{
    [Authorize]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie)]
    public class CategoryController : BaseController
    {
        public CategoryController(IUow uow)
        {
            _Uow = uow;
        }

        [ActionName("GetAll")]
        [HttpGet]
        public IHttpActionResult GetAll(int page = 1, int itemsPerPage = 20, string sortBy = "DisplayOrder", bool reverse = false, string search = null)
        {
            try
            {
                var categories = _Uow._Categories.GetAll(x => x.Active == true);

                // searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    categories = categories.Where(x =>
                        x.CategoryURL.ToLower().Contains(search) ||
                        x.SEOName.ToLower().Contains(search) ||
                        x.CategoryURL.ToLower().Contains(search) ||
                        x.Name.ToLower().Contains(search));
                }

                // sorting (done with the System.Linq.Dynamic library available on NuGet)
                categories = categories.OrderBy(sortBy + (reverse ? " descending" : ""));

                // paging
                var categoriesPaged = categories.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);

                // json result
                var json = new
                {
                    count = _Uow._Categories.Count(),
                    data = categoriesPaged,
                };

                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [ActionName("AddCategory")]
        [HttpPost]
        public async Task<IHttpActionResult> AddCategory(CategoryModel model)
        {
            try
            {
                var category = new DrNajeeb.EF.Category();
                category.Name = model.Name;
                category.IsShowOnFrontPage = model.IsShowOnFrontPage ?? false;
                category.SEOName = Helpers.URLHelpers.URLFriendly(model.Name);
                category.CreatedOn = DateTime.UtcNow;
                category.Active = true;
                var maxValue = await _Uow._Categories.GetAll(x => x.Active == true).OrderByDescending(x => x.DisplayOrder).FirstOrDefaultAsync();
                category.DisplayOrder = (maxValue != null) ? maxValue.DisplayOrder + 1 : 1;

                //todo : add useid in createdby
                //todo : set seo name like QA site
                //todo : set and save category url
                //set display order of categories

                _Uow._Categories.Add(category);
                await _Uow.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("DeleteCategory")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _Uow._Categories.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                category.Active = false;
                category.UpdatedOn = DateTime.Now;
                _Uow._Categories.Update(category);

                //todo : add useid in updatedby

                await _Uow.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("UpdateCategory")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateCategory(CategoryModel model)
        {
            try
            {
                var category = await _Uow._Categories.GetByIdAsync(model.Id);
                category.Name = model.Name;
                category.IsShowOnFrontPage = model.IsShowOnFrontPage ?? false;
                category.SEOName = Helpers.URLHelpers.URLFriendly(model.Name);
                category.UpdatedOn = DateTime.Now;

                //todo : add useid in updatedby

                _Uow._Categories.Update(category);
                await _Uow.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("UpdateOrder")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateOrder(List<CategorySortingModel> model)
        {
            try
            {
                var categories=_Uow._Categories.GetAll(x=>x.Active==true);

                foreach (var item in model)
                {
                    var displayOrder = item.LocationNo + 1;
                    var category = await categories.FirstOrDefaultAsync(x=>x.Id==item.CategoryId);
                    if (category != null)
                    {
                        if (category.DisplayOrder != displayOrder)
                        {
                            category.DisplayOrder = displayOrder;
                            _Uow._Categories.Update(category);
                        }
                    }
                }
                await _Uow.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetSingle")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSingle(int Id)
        {
            try
            {
                var category = await _Uow._Categories.GetByIdAsync(Id);
                if (category == null)
                {
                    return NotFound();
                }

                var model = new CategoryModel();
                model.CategoryURL = category.CategoryURL;
                model.Id = category.Id;
                model.ImageURL = category.ImageURL;
                model.Name = category.Name;
                model.IsShowOnFrontPage = category.IsShowOnFrontPage;
                model.SeoName = category.SEOName;
                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ActionName("GetUserCategories")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetUserCategories()
        {
            try
            {
                var categoriesList = new List<UserCategoryModel>();
                var categories = await _Uow._Categories
                    .GetAll(x => x.Active == true && x.IsShowOnFrontPage == true)
                    .OrderBy(x => x.DisplayOrder)
                    .ToListAsync();
                categories.ForEach(x => categoriesList.Add(new UserCategoryModel
                {
                    Id = x.Id,
                    Name = x.Name
                }));
                return Ok(categoriesList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
