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
    using System.Collections.Generic;
    
    public partial class Order_Cancellation
    {
        public int Cancellation_Id { get; set; }
        public Nullable<int> Order_Id { get; set; }
        public System.DateTime Cancellation_Date { get; set; }
        public Nullable<decimal> Refund_Amount { get; set; }
    
        public virtual Order_Details Order_Details { get; set; }
    }
}
