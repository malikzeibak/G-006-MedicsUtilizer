//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HackathonWeb
{
    using System;
    using System.Collections.Generic;
    
    public partial class RequestFollowUp
    {
        public int ID { get; set; }
        public int RequestID { get; set; }
        public System.DateTime AssignedDateTime { get; set; }
        public int AssignedToID { get; set; }
        public Nullable<System.DateTime> ReceivedDateTime { get; set; }
        public string Status { get; set; }
    
        public virtual Request Request { get; set; }
    }
}