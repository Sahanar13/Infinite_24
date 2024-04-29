using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Question1_mvc_.Models; 

namespace Question1_mvc_.Controllers
{
    public class CodeController : Controller
    {
        private readonly NorthWindEntities db; 

        public CodeController()
        {
            db = new NorthWindEntities(); 
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose(); 
            base.Dispose(disposing);
        }

        public ActionResult GermanyCustomers()
        {
            var germanCustomers = db.Customers.Where(c => c.Country == "Germany").ToList();
            return View(germanCustomers);
        }

       
        public ActionResult CustomerDetailsByOrderId()
        {
            var orderDetails = db.Order_Details.FirstOrDefault(od => od.OrderID == 10248);
            if (orderDetails != null)
            {
                var customer = db.Customers.FirstOrDefault(c => c.CustomerID == orderDetails.CustomerID);
                return View(customer);
            }
            return View();
        }
    }
}
