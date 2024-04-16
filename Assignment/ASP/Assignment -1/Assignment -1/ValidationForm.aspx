<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ValidationForm.aspx.cs" Inherits="Assignment__1.ValidationForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Validator
    </title>
</head>
<body>
    <form id="form1" runat="server">
         <div>
            <h2>Insert your details:</h2>
            <asp:Label ID="Label1" runat="server" Text="Name"></asp:Label>
            :<asp:TextBox ID="txtName" runat="server" placeholder="Name" Height="17px" style="margin-left: 11px" Width="164px" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName" ErrorMessage="Enter your name" ForeColor="Red">*</asp:RequiredFieldValidator>
            <br />
            <br />
            <br />
            <asp:Label ID="Label2" runat="server" Text="Family Name"></asp:Label>
            :
             <asp:TextBox ID="txtFamilyName" runat="server" placeholder="Family Name" Height="16px" Width="164px" />

            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtFamilyName" ErrorMessage="differs from name" ForeColor="Red">*</asp:RequiredFieldValidator>
            <br />
            <br />
            <asp:Label ID="Label3" runat="server" Text="Address"></asp:Label>
            :<asp:TextBox ID="txtAddress" runat="server" placeholder="Address" style="margin-left: 5px" Width="259px" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtAddress" ErrorMessage="atleast 2 char" ForeColor="Red">*</asp:RequiredFieldValidator>
            <br />
            <br />
            <br />
            <asp:Label ID="Label4" runat="server" Text="City"></asp:Label>
            :<asp:TextBox ID="txtCity" runat="server" placeholder="City" style="margin-left: 7px" Width="145px" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtCity" ErrorMessage="atleast 2 char" ForeColor="Red">*</asp:RequiredFieldValidator>
            <br />
            <br />
            <br />
            <asp:Label ID="Label5" runat="server" Text="Zipcode"></asp:Label>
            :<asp:TextBox ID="txtZipCode" runat="server" placeholder="Zip Code" style="margin-left: 6px" Width="116px" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtZipCode" ErrorMessage="(xxxxxx)" ForeColor="Red">*</asp:RequiredFieldValidator>
            <br />
            <br />
            <br />
            <asp:Label ID="Label6" runat="server" Text="Phone No"></asp:Label>
            :<asp:TextBox ID="txtPhone" runat="server" placeholder="Phone" style="margin-left: 7px" Width="166px" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtPhone" ErrorMessage="(xx-xxxxxxxxxx)" ForeColor="Red">*</asp:RequiredFieldValidator>
            <br />
            <br />
            <asp:Label ID="Label7" runat="server" Text="E-Mail"></asp:Label>
            :<asp:TextBox ID="txtEmail" runat="server" placeholder="E-Mail" style="margin-left: 9px" Width="255px" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtEmail" ErrorMessage="ecample@example.com" ForeColor="Red">*</asp:RequiredFieldValidator>
            <br />
            <br />
            <br />
             <asp:ValidationSummary ID="ValidationSummary1" runat="server" HeaderText="Validation Errors" ShowMessageBox="true" />
           <asp:Button ID="btnCheck" runat="server" Text="Check" OnClick="btnCheck_Click" />

        </div>
    </form>
</body>
</html>