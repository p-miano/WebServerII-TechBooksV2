using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using TechBooks.Data.ADO.Net;
using TechBooks.Models;

namespace TechBooks.WebUI.Controllers
{
    [Authorize]
    public class ManageCategoriesController : CommonBaseClass
    {
        // GET: ManageCategories
        public ActionResult Index()
        {
            var listOfCategories = new List<Category>();
            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    listOfCategories = CategoriesData.GetList(cn);
                }
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
            }
            return View(listOfCategories);
        }

        public ActionResult AddOrUpdate(int? id)
        {
            if (id == null) return View();

            Category category = null;
            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    category = CategoriesData.GetCategory((int)id, cn);
                    if (category == null)
                        return RedirectToAction("Index", "NotFound", new { entity = "Category", backUrl = "/ManageCategories/" });
                }
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrUpdate(Category category)
        {
            if (!ModelState.IsValid)
                return (category.CategoryId == 0) ? View() : View(category);

            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    if (category.CategoryId == 0)
                        CategoriesData.Insert(category, cn);
                    else
                        CategoriesData.Update(category, cn);
                }
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
                return (category.CategoryId == 0) ? View() : View(category);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(Category category)
        {
            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    if (CategoriesData.HasBooks(category, cn))
                        throw new Exception("This Category cannot be removed because it has been associated with one or more books. Remove all associations first.");
                    else
                        CategoriesData.Delete(category, cn);
                }
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}