using Library.Models;
using System;
using System.Web.Mvc;
using static Library.Classes.SortData;
using Library.Classes;
using System.Collections.Generic;

namespace Library.Controllers
{
    public class UserController : Controller
    {
        private DataOperations dataOperations = new DataOperations();


        public ActionResult Index(int page = 1)
        {
            int pageSize = 5;
            BooksAuthorsModel booksAuthorsModel = dataOperations.GetAllBooks(page, pageSize, SortQuery, FilterQuery, User.Identity.Name);
            PageInfo pageInfo = new PageInfo() { PageNumber = page, PageSize = pageSize, TotalItems = dataOperations.GetCountRows() };
            booksAuthorsModel.PageInfo = pageInfo;
            return View(booksAuthorsModel);
        }


        [HttpGet]
        [Authorize]
        public ActionResult TakeBook(int id)
        {
            return View(dataOperations.Find(id));
        }


        [HttpPost, ActionName("TakeBook")]
        public ActionResult TakeConfirmed(int id)
        {
            IEnumerable<User> Users = dataOperations.GetUsersTakenBook(id);
            foreach(User user in Users)
            {
                if (user.Us_name == User.Identity.Name)
                {
                    ModelState.AddModelError("Book_name", "You have already taken this book!");
                    return View(dataOperations.Find(id));
                }
            }
            dataOperations.ChangeQuantity(id);
            dataOperations.AddTakenBook(id, User.Identity.Name, DateTime.Today);
            EmailClass mail = new EmailClass();
            Book book = dataOperations.Find(id);
            mail.SendMessage(User.Identity.Name, book.Book_name);
            return RedirectToAction("Index");
        }


        public ActionResult BookSort()
        {
            BookSortParam++;
            SortQuery = " order by Book_name ";
            if (BookSortParam % 2 == 0)
            {
                SortQuery += " desc ";
            }
            return RedirectToAction("Index");
        }


        public ActionResult AuthorSort()
        {
            AuthorSortParam++;
            SortQuery = " order by Author_name ";
            if (AuthorSortParam % 2 == 0)
            {
                SortQuery += " desc ";
            }
            return RedirectToAction("Index");
        }


        public ActionResult AllBooksFilter()
        {
            FilterQuery = "";
            return RedirectToAction("Index");
        }


        public ActionResult AvailableBooksFilter()
        {
            FilterQuery = " and Quantity > 0 ";
            return RedirectToAction("Index");
        }


        public ActionResult MyBooksFilter()
        {
            FilterQuery = "UserBooks";
            return RedirectToAction("Index");
        }
    }
}