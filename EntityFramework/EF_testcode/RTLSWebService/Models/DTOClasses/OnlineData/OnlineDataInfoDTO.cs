using System.Collections.Generic;

namespace RTLSWebService.Models.DTOClasses.OnlineData
{
    public class OnlineDataInfoDTO
    {
        public string P_ID { get; set; }
        public int T_ID { get; set; }
        public int Z_ID { get; set; }
        public string LOC { get; set; }
        public int STATUS { get; set; }
        public decimal T_BAT_SIG_STR { get; set; }
        public decimal B_BAT_SIG_STR { get; set; }
        public decimal T_BAT_STA_VAL { get; set; }
        public int T_BAT_STA_PERCNT { get; set; }
        public decimal BS_BAT_STA_VAL { get; set; }
        public int BS_BAT_STA_PER { get; set; }
        public string IN_TIME { get; set; }
        public string ALL_NOT_TME { get; set; }
        public string P_TME { get; set; }
        public string NO_M_TME { get; set; }
        public string OUT_TME { get; set; }
        public string TEMP_TME { get; set; }
        public string LOW_BAT_TME { get; set; }
        public string FOUT_TME { get; set; }
        public string LAST_UPDATE_TIME { get; set; }
        public int NO_OF_OUT { get; set; }
        public int NO_OF_PANIC { get; set; }
        public int NO_OF_IN_ACTIVE { get; set; }
        public int NO_OF_TEMP { get; set; }
        public int NO_OF_LBAT { get; set; }
        public decimal TEMP_VAL { get; set; }   
        public List<int> LOCS { get; set; }
        public OnlineDataInfoDTO()
        {
            LOCS = new List<int>();
        }

    }
}