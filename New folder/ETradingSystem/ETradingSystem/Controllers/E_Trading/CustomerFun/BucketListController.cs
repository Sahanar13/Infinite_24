// BucketListController.cs
using ETradingSystem.Models;
using System.Linq;
using System.Web.Mvc;

namespace ETradingSystem.Controllers.E_Trading.CustomerFun
{
    public class BucketListController : Controller
    {
        private readonly E_TradingDBEntities2 db;

        public BucketListController()
        {
            db = new E_TradingDBEntities2();
        }

        // GET: BucketList
        public ActionResult Index()
        {
            if (Session["CustomerEmail"] == null)
            {
                return RedirectToAction("Login", "CustomerValidation");
            }

            string customerEmail = Session["CustomerEmail"].ToString();
            var customer = db.Customers.FirstOrDefault(c => c.Customer_Email == customerEmail);

            if (customer != null)
            {
                var bucketListItems = db.BucketLists.Where(b => b.Customer_Id == customer.Customer_Id).ToList();
                return View(bucketListItems);
            }
            else
            {
                return RedirectToAction("Login", "CustomerValidation");
            }
        }

        // Action to add a product to the bucket list
        // Action to add a product to the bucket list
        public ActionResult AddToBucketList(int productId)
        {
            if (Session["CustomerEmail"] == null)
            {
                // Store the attempted URL in TempData for redirection after login
                TempData["ReturnUrl"] = Url.Action("Details", "Products", new { id = productId });

                // Redirect to login page
                return RedirectToAction("Login", "CustomerValidation");
            }

            // Retrieve customer email from session
            var customerEmail = Session["CustomerEmail"].ToString();

            // Retrieve customer details based on email
            var customer = db.Customers.FirstOrDefault(c => c.Customer_Email == customerEmail);

            // Retrieve product details based on id
            var product = db.Products.Find(productId);

            if (customer != null && product != null)
            {
                // Create a new BucketList item and add it to the customer's bucket list
                var bucketListItem = new BucketList
                {
                    Customer_Id = customer.Customer_Id,
                    Product_Id = product.Product_Id,
                    // Assign any other required properties of BucketList
                };

                // Add the bucket list item to the database
                db.BucketLists.Add(bucketListItem);
                db.SaveChanges();

                // Display success message
                TempData["SuccessMessage"] = "Product added to bucket list successfully.";
            }

            // Redirect back to the product details page
            return RedirectToAction("Details", "Products", new { id = productId });
        }


        public ActionResult PlaceOrder()
        {
            if (Session["CustomerEmail"] == null)
            {
                return RedirectToAction("Login", "CustomerValidation");
            }

            // Place order logic
            // You can implement order placement here based on the items in the bucket list

            return RedirectToAction("Index", "Orders");
        }
    }
}
