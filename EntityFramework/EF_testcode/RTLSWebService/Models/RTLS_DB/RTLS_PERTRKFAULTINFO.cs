//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RTLSWebService.Models.RTLS_DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class RTLS_PERTRKFAULTINFO
    {
        public decimal FLTNO { get; set; }
        public string fltname { get; set; }
        public string FLTDESC { get; set; }
        public string FLTLEVEL { get; set; }
        public Nullable<System.DateTime> CREATEDTIME { get; set; }
        public Nullable<decimal> OPERATION_TYPE { get; set; }
    }
}