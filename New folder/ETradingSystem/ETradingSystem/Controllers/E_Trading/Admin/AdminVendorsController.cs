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
    public class AdminVendorsController : Controller
    {
        private E_TradingDBEntities2 db = new E_TradingDBEntities2();

        public ActionResult Index()
        {
            var vendors = db.Vendors.Include(v => v.Hint);
            return View(vendors.ToList());
        }
        public ActionResult GetVendorsByVendorName(string vendorName)
        {
            var vendors = db.Vendors.Where(v => v.Vendor_Name == vendorName).ToList();

            return View("Index", vendors);
        }
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return View(vendor);
        }

        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return View(vendor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Vendor vendor = db.Vendors.Find(id);
            if (vendor != null)
            {
                if (vendor.Status == "Active")
                {
                    vendor.Status = "InActive";
                    db.SaveChanges();
                }
                else
                {
                    vendor.Status = "Active";
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
