using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using TechBooks.Data.ADO.Net;
using TechBooks.Models;
using TechBooks.WebUI.Models;

namespace TechBooks.WebUI.Controllers
{
    [Authorize]
    public class ManageAuthorBooksController : CommonBaseClass
    {
        // GET: ManageAuthorBooks
        public ActionResult Index(int? id)
        {
            if (id == null) return RedirectToAction("Index", "NotFound", new { entity = "Author", backUrl = "/ManageAuthors/" });

            Author author = null;
            List<Book> listOfAssociatedBooks = new List<Book>();
            List<Book> listOfNonAssociatedBooks = new List<Book>();

            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    author = AuthorsData.GetAuthor((int)id, cn);
                    if (author == null) return RedirectToAction("Index", "NotFound", new { entity = "Author", backUrl = "/ManageAuthors/" });
                    listOfAssociatedBooks = AuthorBooksData.GetAssociatedBookList((int)id, cn);
                    listOfNonAssociatedBooks = AuthorBooksData.GetNonAssociatedBookList((int)id, cn);
                }
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
            }

            var myViewModel = new AuthorBooksViewModel();
            myViewModel.Author = author;
            myViewModel.AssociatedBooks = listOfAssociatedBooks;
            myViewModel.NonAssociatedBooks = listOfNonAssociatedBooks;

            return View(myViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int authorId, int bookId)
        {
            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    AuthorBooksData.Insert(authorId, bookId, cn);
                }
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index), new { id = authorId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(int authorId, int bookId)
        {
            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    AuthorBooksData.Delete(authorId, bookId, cn);
                }
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index), new { id = authorId });
        }
    }
}