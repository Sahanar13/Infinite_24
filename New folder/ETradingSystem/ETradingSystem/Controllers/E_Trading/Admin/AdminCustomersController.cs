using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ETradingSystem.Models;

namespace ETradingSystem.Controllers.E_Trading.Admin
{
    public class AdminCustomersController : Controller
    {
        private readonly E_TradingDBEntities2 db;
        public AdminCustomersController()
        {
            db = new E_TradingDBEntities2();
        }

        public ActionResult Index()
        {
            var customers = db.Customers.Include(c => c.Hint);
            return View(customers.ToList());
        }
        public ActionResult GetCustomerByCustomerName(string customerName)
        {
            var vendors = db.Customers.Where(v => v.Customer_Name == customerName).ToList();

            return View("Index", vendors);
        }
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Customer customer = db.Customers.Find(id);
            if (customer != null)
            {
                if (customer.Status == "Active")
                {
                    customer.Status = "InActive";
                    db.SaveChanges();
                }
                else
                {
                    customer.Status = "Active";
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
