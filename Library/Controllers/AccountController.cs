using Library.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace Library.Controllers
{
    public class AccountController : Controller
    {
        private UserOperations userOperations = new UserOperations();


        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Register(AccountUser user)
        {
            AccountUser user1 = new AccountUser();
            user1 = userOperations.FindUser(user.UserName);
            if (user1.UserName == "empty")
            {
                int reg = userOperations.AddUser(user);
                if (reg != 0)
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, true);
                    if (user.UserRoots == "user")
                    {
                        return RedirectToAction("Index", "User");
                    }
                    if (user.UserRoots == "admin")
                    {
                       return RedirectToAction("Index", "Admin");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User " + user.UserName + " exists! Try to write other name.");
            }
            return View();
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(AccountUser user)
        {
            AccountUser user1 = new AccountUser();
            user1 = user;
            user = userOperations.FindUser(user.UserName);
            if (user.UserName != "empty")
            {
                FormsAuthentication.SetAuthCookie(user.UserName, true);
                if (user.UserRoots == "user")
                {
                    return RedirectToAction("Index", "User");
                }
                if (user.UserRoots == "admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            else
            {
                ModelState.AddModelError("", "User " + user1.UserName + " not exists!");
            }
            return View();
        }
    }
}