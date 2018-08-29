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
    
    public partial class RTLS_ONLINEPERSONSTATUS
    {
        public string PERSONID { get; set; }
        public Nullable<decimal> TAGID { get; set; }
        public Nullable<decimal> Zone_ID { get; set; }
        public Nullable<decimal> status { get; set; }
        public Nullable<decimal> TAG_SIGNALSTRENGTH { get; set; }
        public Nullable<decimal> BS_SIGNALSTRENGTH { get; set; }
        public Nullable<System.DateTime> INTIME { get; set; }
        public Nullable<System.DateTime> ALLISNOTWELLTIME { get; set; }
        public Nullable<System.DateTime> PANICTIME { get; set; }
        public Nullable<System.DateTime> NOMOTIONTIME { get; set; }
        public Nullable<System.DateTime> OUT_TIME { get; set; }
        public Nullable<System.DateTime> FOUT_TIME { get; set; }
        public Nullable<System.DateTime> TEMPEXCEEDTIME { get; set; }
        public Nullable<System.DateTime> LOW_BATTERY_TIME { get; set; }
        public Nullable<System.DateTime> LASTUPDATEDTIME { get; set; }
        public Nullable<decimal> TAG_BATTERY_STATUS_VAL { get; set; }
        public Nullable<decimal> TAG_BATTERY_STATUS_PERCNT { get; set; }
        public Nullable<decimal> BS_BATTERY_STATUS_VAL { get; set; }
        public Nullable<decimal> BS_BATTERY_STATUS_PERCNT { get; set; }
        public System.DateTime created_time { get; set; }
        public Nullable<decimal> TEMP_VALUE { get; set; }
    
        public virtual RTLS_PERSONDTLS RTLS_PERSONDTLS { get; set; }
    }
}