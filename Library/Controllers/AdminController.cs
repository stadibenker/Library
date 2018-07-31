using Library.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using static Library.Classes.SortData;

namespace Library.Controllers
{
    public class AdminController : Controller
    {
        private DataOperations dataOperations = new DataOperations();
        private UserOperations userOperations = new UserOperations();


        [Authorize]
        public ActionResult Index(int page = 1) 
        {
            if (userOperations.Authentication(User.Identity.Name) == true)
            {
                int pageSize = 5;
                BooksAuthorsModel booksAuthorsModel = dataOperations.GetAllBooks(page, pageSize, SortQuery, FilterQuery, "");
                PageInfo pageInfo = new PageInfo() { PageNumber = page, PageSize = pageSize, TotalItems = dataOperations.GetCountRows() };
                booksAuthorsModel.PageInfo = pageInfo;
                return View(booksAuthorsModel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpGet]
        [Authorize]
        public ActionResult Add()
        {
            if (userOperations.Authentication(User.Identity.Name) == true)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost]
        public ActionResult Add(Book book)
        {
            if (dataOperations.Find(book.Book_id) == null)
            {
                dataOperations.Add(book);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("Book_id", " Book with id " + book.Book_id + " exists! Enter other id!");
            }
            return View();
        }


        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            if (userOperations.Authentication(User.Identity.Name) == true)
            {
                return View(dataOperations.Find(id));
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost]
        public ActionResult Edit(Book book)
        {
            dataOperations.Delete(book.Book_id);
            dataOperations.Add(book);
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Authorize]
        public ActionResult Delete (int id)
        {
            if (userOperations.Authentication(User.Identity.Name) == true)
            {
                return View(dataOperations.Find(id));
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            dataOperations.Delete(id);
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


        [Authorize]
        public ActionResult History(int id)
        {
            if (userOperations.Authentication(User.Identity.Name) == true)
            {
                IEnumerable<User> Users = dataOperations.GetUsersTakenBook(id);
                Book book = dataOperations.Find(id);
                UserBookModel userBookModel = new UserBookModel { Users = Users, book = book};
                return View(userBookModel);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}