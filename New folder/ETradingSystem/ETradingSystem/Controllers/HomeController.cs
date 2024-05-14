using ETradingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ETradingSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Help()
        {
            ViewBag.Messenge = "Help page";
            return View();
        }
        public ActionResult Welcome()
        {
            ViewBag.Messenge = "Welcome page";
            return View();
        }
        public ActionResult Login()
        {
            ViewBag.Messenge = "Login page";
            return View();
        }
        public ActionResult AboutUs()
        {
            ViewBag.Messenge = "AboutUS page";
            return View();
        }
        public ActionResult BucketList()
        {
            if (Session["CustomerEmail"] == null)
            {
                return RedirectToAction("Login", "CustomerValidation");
            }

            // Redirect to Bucket List Controller
            return RedirectToAction("Index", "BucketList");
        }

        // Action to search products
        // Action to search products
        public ActionResult Search(string searchTerm)
        {
            List<Product> products = new List<Product>();

            // Define your connection string
            string connectionString = " Server = ICS-LT-9R368G3\\SQLEXPRESS; Database = E_TradingDB; Integrated Security = True; ";


            // Define your SQL query
            string query = "SELECT * FROM Products WHERE Product_Name LIKE '%' + @SearchTerm + '%'";

            // Create a connection to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create a command object with the SQL query and connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to the query
                    command.Parameters.AddWithValue("@SearchTerm", searchTerm);

                    // Open the connection
                    connection.Open();

                    // Execute the command and read the results
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Iterate through the results and add them to the products list
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                Product_Id = Convert.ToInt32(reader["Product_Id"]),
                                Product_Name = reader["Product_Name"].ToString(),
                                Brand = reader["Brand"].ToString(),
                                Color = reader["Color"].ToString(),
                                Price = Convert.ToDouble(reader["Price"]),
                                Available_Stock = Convert.ToInt32(reader["Available_Stock"]),
                                ImageFileName = reader["ImageFileName"].ToString(),
                                // Add other properties as needed
                            };
                            products.Add(product);
                        }
                    }
                }
            }

            // Pass the search results to the view
            return View(products);
        }
        public ActionResult AddToBucketList()
        {
            // Logic to add the product to the bucket list
            // Redirect to the BucketList controller's Index action
            return RedirectToAction("Index", "BucketList");
        }
        public ActionResult BuyNow(int productId)
        {
            // Logic to handle buying a product
            // You can add code here to process the purchase

            return RedirectToAction("OrderConfirmation", "Home");
        }

        // GET: Home/OrderConfirmation
        public ActionResult OrderConfirmation(Order_Details orderDetails)
        {
            return View(orderDetails);
        }

        public ActionResult OrderBookedSuccessfully()
        {
            return View();
        }

    }
}
