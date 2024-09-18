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
    public class ManageBookAuthorsController : CommonBaseClass
    {
        // GET: ManageBookAuthors
        public ActionResult Index(int? id)
        {
            if (id == null) return RedirectToAction("Index", "NotFound", new { entity = "Book", backUrl = "/ManageBooks/" });

            Book book = null;
            List<Author> listOfAssociatedAuthors = new List<Author>();
            List<Author> listOfNonAssociatedAuthors = new List<Author>();

            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    book = BooksData.GetBook((int)id, cn);
                    if (book == null) return RedirectToAction("Index", "NotFound", new { entity = "Book", backUrl = "/ManageBooks/" });
                    listOfAssociatedAuthors = AuthorBooksData.GetAssociatedAuthorList((int)id, cn);
                    listOfNonAssociatedAuthors = AuthorBooksData.GetNonAssociatedAuthorList((int)id, cn);
                }
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
            }

            var myViewModel = new BookAuthorsViewModel();
            myViewModel.Book = book;
            myViewModel.AssociatedAuthors = listOfAssociatedAuthors;
            myViewModel.NonAssociatedAuthors = listOfNonAssociatedAuthors;

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
            return RedirectToAction(nameof(Index), new { id = bookId });
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
            return RedirectToAction(nameof(Index), new { id = bookId });
        }
    }
}