//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ETradingSystem.Models
{
    using System;
    
    public partial class GetProductsByVendorId_Result
    {
        public decimal Product_Id { get; set; }
        public string Product_Name { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public Nullable<double> Price { get; set; }
        public Nullable<int> Available_Stock { get; set; }
        public string Status { get; set; }
        public string ImageFileName { get; set; }
        public string isdeleted { get; set; }
        public string Vendor_Name { get; set; }
    }
}