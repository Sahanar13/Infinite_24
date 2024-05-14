using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ETradingSystem.Models;
namespace ETradingSystem.Controllers.E_Trading.CustomerFun
{
    public class CustomerValidationController : Controller
    {
      
        private readonly E_TradingDBEntities2 db; 

        public CustomerValidationController()
        {
            db = new E_TradingDBEntities2(); 
        }

        public string Cust_email { get; private set; }

        // GET: Customer
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (IsValidCustomer(email, password))
            {
                Cust_email = email;
                Session["CustomerEmail"] = email; // Store customer email in session

                // Check if there's a return URL stored in TempData
                var returnUrl = TempData["ReturnUrl"] as string;
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.InvalidLogin = "Invalid Customer Email or password.";
                return View();
            }
        }




        private bool IsValidCustomer(string email, string password)
        {
            string Email = db.Customers.Where(x => x.Customer_Email == email).Select(x => x.Customer_Email).FirstOrDefault();
            string Password = db.Customers.Where(x => x.Customer_Email == email).Select(x => x.Password).FirstOrDefault();
            if (email == Email && password == Password)
            {
                return true;
            }
            return false;
        }
    }
}