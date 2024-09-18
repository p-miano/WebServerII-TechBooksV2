using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using TechBooks.Data.ADO.Net;
using TechBooks.Models;

namespace TechBooks.WebUI.Controllers
{
    [Authorize]
    public class ManageBooksController : CommonBaseClass
    {
        #region Support Methods
        private void LoadViewBag_Categories(SqlConnection cn = null)
        {
            List<Category> listOfCategories = new List<Category>();
            if (cn == null)
            {
                try
                {
                    using (cn = new SqlConnection(ConnectionString))
                    {
                        listOfCategories = CategoriesData.GetList(cn);
                    }
                }
                catch (Exception ex)
                {
                    TempData["DangerMessage"] = ex.Message;
                }
            }
            else
                listOfCategories = CategoriesData.GetList(cn);
            ViewBag.Categories = new SelectList(listOfCategories, "CategoryId", "Description");
        }
        #endregion

        // GET: ManageBooks
        public ActionResult Index()
        {
            var listOfBooks = new List<Book>();
            var listOfCategories = new List<Category>();
            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    listOfCategories = CategoriesData.GetList(cn);
                    listOfBooks = BooksData.GetList(cn);
                }
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
            }

            foreach (var book in listOfBooks)
            {
                book.Category = listOfCategories.FirstOrDefault(c => c.CategoryId == book.CategoryId);
            }

            return View(listOfBooks);
        }

        public ActionResult AddOrUpdate(int? id)
        {
            Book book = null;
            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    LoadViewBag_Categories(cn);
                    if (id == null) return View();
                    book = BooksData.GetBook((int)id, cn);
                    if (book == null)
                        return RedirectToAction("Index", "NotFound", new { entity = "Book", backUrl = "/ManageBooks/" });
                }
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrUpdate(Book book)
        {
            if (!ModelState.IsValid)
            {
                LoadViewBag_Categories();
                return (book.BookId == 0) ? View() : View(book);
            }

            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    if (book.BookId == 0)
                        BooksData.Insert(book, cn);
                    else
                        BooksData.Update(book, cn);
                }
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
                LoadViewBag_Categories();
                return (book.BookId == 0) ? View() : View(book);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(Book book)
        {
            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    if (BooksData.HasAuthors(book, cn))
                        throw new Exception("This Book cannot be removed because it has been associated with one or more authors. Remove all associations first.");
                    else
                        BooksData.Delete(book, cn);
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
