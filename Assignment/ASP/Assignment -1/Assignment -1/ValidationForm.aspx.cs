using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Assignment__1
{
    public partial class ValidationForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            // Perform validation
            string name = txtName.Text;
            string familyName = txtFamilyName.Text;
            string address = txtAddress.Text;
            string city = txtCity.Text;
            string zipCode = txtZipCode.Text;
            string phone = txtPhone.Text;
            string email = txtEmail.Text;

            bool isValid = true;
            string errorMessage = "";

            // Validation rules

            if (name.Equals(familyName, StringComparison.OrdinalIgnoreCase))
            {
                isValid = false;
                errorMessage += "Name must be different from Family Name.\n";
            }

            if (address.Length < 2)
            {
                isValid = false;
                errorMessage += "Address must be at least 2 characters long.\n";
            }

            if (city.Length < 2)
            {
                isValid = false;
                errorMessage += "City must be at least 2 characters long.\n";
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(zipCode, @"^\d{5}$"))
            {
                isValid = false;
                errorMessage += "Zip Code must be 5 digits.\n";
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{2,3}-\d{7}$"))
            {
                isValid = false;
                errorMessage += "Phone must be in the format XX-XXXXXXX or XXX-XXXXXXX.\n";
            }

            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
            }
            catch (FormatException)
            {
                isValid = false;
                errorMessage += "Invalid email address.\n";
            }

            if (isValid)
            {
                // Redirect to success page or perform further actions
                Response.Redirect("SuccessPage.aspx");
            }
            else
            {
                // Display error message
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{errorMessage}');", true);
            }
            if (isValid)
            {
                // Redirect to HtmlPage1
                Response.Redirect("Welcome.html");
            }
        }
    }
   
}
          
