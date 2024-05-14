using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ETradingSystem.Models;
namespace ETradingSystem.Controllers.E_Trading.Admin
{
    public class AdminValidationController : Controller
    {
        private readonly E_TradingDBEntities2 db; 

        public AdminValidationController()
        {
            db = new E_TradingDBEntities2(); 
        }
        // GET: Admin
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (IsValidAdmin(email, password))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewBag.InvalidLogin = "Invalid Admin Email or Password.";
                return View();
            }
        }

        private bool IsValidAdmin(string email, string password)
        {
            string Email = db.Admins.Where(x => x.Admin_Email == email).Select(x => x.Admin_Email).FirstOrDefault();
            string Password = db.Admins.Where(x => x.Admin_Email == email).Select(x => x.Password).FirstOrDefault();
            if (email == Email && password == Password)
            {
                return true;
            }
            return false;
        }
    }
}