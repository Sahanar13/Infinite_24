using ETradingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ETradingSystem.Controllers.E_Trading.CustomerFun
{
    public class CustomersController : Controller
    {
        // GET: Customers
        private readonly E_TradingDBEntities2 db;
        public CustomersController()
        {
            db = new E_TradingDBEntities2();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            ViewBag.Hint_Id = new SelectList(db.Hints, "Hint_Id", "Questions");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Customer_Id,Customer_Name,Customer_Email,Date_Of_Birth,Address,Balance,Mobile_Number,Password,Hint_Id,Hint_Answer,Status")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.Status = "Active";
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Hint_Id = new SelectList(db.Hints, "Hint_Id", "Questions", customer.Hint_Id);
            return View(customer);
        }
    }
}