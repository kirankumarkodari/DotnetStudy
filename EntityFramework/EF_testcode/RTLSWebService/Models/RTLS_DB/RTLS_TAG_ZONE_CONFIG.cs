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
    
    public partial class RTLS_TAG_ZONE_CONFIG
    {
        public decimal tagid { get; set; }
        public decimal zone_id { get; set; }
        public System.DateTime created_time { get; set; }
        public Nullable<decimal> opeartion_type { get; set; }
    
        public virtual RTLS_ZONEDTLS RTLS_ZONEDTLS { get; set; }
    }
}