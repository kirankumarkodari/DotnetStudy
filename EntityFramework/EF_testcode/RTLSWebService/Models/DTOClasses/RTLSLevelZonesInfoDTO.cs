using System.Collections.Generic;

namespace RTLSWebService.Models.DTOClasses
{
    public class RTLSLevelZonesInfoDTO
    {
        public List<int> L1 { get; set; }
        public List<int> L2 { get; set; }
        public List<int> L3 { get; set; }

        public RTLSLevelZonesInfoDTO()
        {

            L1 = new List<int>();

            L2 = new List<int>();
            L3 = new List<int>();

        }
    }
}