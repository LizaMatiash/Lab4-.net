using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Lab4.Models;


namespace Lab4.Controllers
{
    public class HomeController : Controller
    {
        ///////////// INIT //////////

        private Models.Service.EFUnitOfWork unitOfWork;

        public HomeController() : base()
        {
            unitOfWork = new Models.Service.EFUnitOfWork();
        }

        public new void Dispose()
        {
            unitOfWork.Dispose();
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }


        ///////////// WEB METHODS //////////      

        public ActionResult Index()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        [HttpGet]
        public ActionResult LoginUser()
        {
            ViewBag.Message = "Login page.";
            return View();
        }

        [HttpPost]
        public ActionResult LoginUser(string login, string password)
        {
            string result = Login(login, password);
            if (Session["CurrentUser"] != null)
            {
                return this.Content(AlertNRedirect(result, "/Home/Index"));
            }
            else
            {
                return this.Content(AlertNRedirect(result, "/Home/LoginUser"));
            }
        }

        public ActionResult LogoutUser()
        {
            string result = Logout();
            return this.Content(AlertNRedirect(result, "/Home/Index"));
        }

        [HttpGet]
        public ActionResult RegisterUser()
        {
            ViewBag.Message = "Register page.";
            return View();
        }

        [HttpPost]
        public ActionResult RegisterUser(string login, string password, string nickname)
        {
            string result = BLL.BLL.Register(login, nickname, password);
            if (result.StartsWith("Користувача "))
            {
                return this.Content(AlertNRedirect(result, "/Home/Index"));
            }
            else
            {
                return this.Content(AlertNRedirect(result, "/Home/RegisterUser"));
            }
        }

        [HttpGet]
        public ActionResult InviteUser()
        {
            ViewBag.Message = "Invite page.";
            return View();
        }

        [HttpPost]
        public ActionResult InviteUser(string nickname)
        {
            string result = BLL.BLL.Invite(nickname, Cast((User)Session["CurrentUser"]));
            return this.Content(AlertNRedirect(result, "/Home/InviteUser"));
        }

        [HttpGet]
        public ActionResult InviteMeUser()
        {
            ViewBag.Message = "Invite me page.";
            return View();
        }

        [HttpPost]
        public ActionResult InviteMeUser(string nickname)
        {
            string result = BLL.BLL.InviteMe(nickname, Cast((User)Session["CurrentUser"]));
            return this.Content(AlertNRedirect(result, "/Home/InviteMeUser"));
        }

        [HttpGet]
        public ActionResult AcceptUser()
        {
            ViewBag.Message = "Accept page.";
            ViewBag.ListInvitesToMe = BLL.BLL.GetAccepts(Cast((User)Session["CurrentUser"]));
            ViewBag.ListInvitesMe = BLL.BLL.GetAcceptsMe(Cast((User)Session["CurrentUser"]));
            return View();
        }

        [HttpPost]
        public ActionResult AcceptUser(string nickname)
        {
            string result = BLL.BLL.Accept(nickname, Cast((User)Session["CurrentUser"]));
            return this.Content(AlertNRedirect(result, "/Home/AcceptUser"));
        }

        [HttpPost]
        public ActionResult AcceptMeUser(string nickname)
        {
            string result = BLL.BLL.AcceptMe(nickname, Cast((User)Session["CurrentUser"]));
            return this.Content(AlertNRedirect(result, "/Home/AcceptUser"));
        }

        public ActionResult FriendsUser()
        {
            ViewBag.Message = "Friends page.";
            ViewBag.MyFriends = BLL.BLL.GetFriends(Cast((User)Session["CurrentUser"]));
            ViewBag.FriendsMe = BLL.BLL.GetFriendsMe(Cast((User)Session["CurrentUser"]));
            return View();
        }

        [HttpGet]
        public ActionResult SelectDialogUser()
        {
            ViewBag.Message = "Friends page.";
            List<string> friends = BLL.BLL.GetFriends(Cast((User)Session["CurrentUser"]));
            friends.AddRange(BLL.BLL.GetFriendsMe(Cast((User)Session["CurrentUser"])));
            friends = friends.Distinct().ToList();
            ViewBag.Friends = friends;
            return View();
        }

        [HttpPost]
        public ActionResult SelectDialogUser(string nickname)
        {
            Session.Add("DialogNicname", nickname);
            return Redirect("~/Home/ShowDialogUser");
        }

        [HttpGet]
        public ActionResult ShowDialogUser()
        {
            ViewBag.Message = "Friends page.";
            List<string> dialog = BLL.BLL.DialogMessages(Session["DialogNicname"].ToString(), Cast((User)Session["CurrentUser"]));
            string result = dialog.First();
            if (result == "true")
            {
                dialog.RemoveAt(0);
                ViewBag.Dialog = dialog;
                return View();
            }
            else
            {
                return this.Content(AlertNRedirect(result, "/Home/ShowDialogUser"));
            }
        }

        [HttpPost]
        public ActionResult ShowDialogUser(string messageBody)
        {
            string result = BLL.BLL.Send(Session["DialogNicname"].ToString(), messageBody, Cast((User)Session["CurrentUser"]));
            return Content(AlertNRedirect(result, "/Home/ShowDialogUser"));
        }


        [HttpGet]
        public ActionResult SendMessageUser()
        {
            ViewBag.Message = "Friends page.";
            List<string> friends = BLL.BLL.GetFriends(Cast((User)Session["CurrentUser"]));
            friends.AddRange(BLL.BLL.GetFriendsMe(Cast((User)Session["CurrentUser"])));
            friends = friends.Distinct().ToList();
            ViewBag.Friends = friends;
            ViewBag.Dialog = new List<string>() { "" };
            ViewBag.Nickname = "";
            return View();
        }

        [HttpPost]
        public ActionResult SendMessageUser(string messageBody, string nickname)
        {
            string result = BLL.BLL.Send(nickname, messageBody, Cast((User)Session["CurrentUser"]));
            return Content(AlertNRedirect(result, "/Home/SendMessageUser"));
        }


        ///////////// SERVICE METHODS //////////

        private string MessageBoxShow(string message)
        {
            return "<script language='javascript' type='text/javascript'>alert('" + message + "');</script>";
        }

        private string AlertNRedirect(string message, string url)
        {
            return "<script language='javascript' type='text/javascript'>alert('" + message + "'); window.location.href = '" + url + "';</script>";
        }

        private string Login(string login, string password)
        {
            DAL.Entities.User user = new DAL.Entities.User();
            string result = BLL.BLL.Login(login, password,out user);
            if (result.StartsWith("Ласкаво просимо, "))
            {
                Session.Add("CurrentUser", Cast(user));
            }
            return result;
        }

        private string Logout()
        {
            if (Session["CurrentUser"] == null)
            {
                return "Щоб вийти, потрібно спочатку увійти.";
            }
            else
            {
                Session.Remove("CurrentUser");
                return "Ви вийшли зі свого аккаунта.";
            }
        }

        private User Cast(DAL.Entities.User duser)
        {
            return new User() { ID = duser.ID, Login = duser.Login, Nickname = duser.Nickname, Password = duser.Password };
        }


        private DAL.Entities.User Cast(User duser)
        {
            return new DAL.Entities.User() { ID = duser.ID, Login = duser.Login, Nickname = duser.Nickname, Password = duser.Password };
        }

    }
}