//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MiniProject_1
{
    using System;
    using System.Collections.Generic;
    
    public partial class Train
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Train()
        {
            this.Bookings = new HashSet<Booking>();
        }
    
        public int TrainId { get; set; }
        public string Class { get; set; }
        public string TrainName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Name { get; set; }
        public Nullable<int> TotalBerths { get; set; }
        public Nullable<int> AvailableBerths { get; set; }
        public Nullable<decimal> Fare { get; set; }
        public Nullable<bool> IsActive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}