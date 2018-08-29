using RTLSWebService.Models.DTOClasses.MasterData;
using System.Collections.Generic;

namespace RTLSWebService.Models.DTOClasses
{
    public class RTLSZonesInfoDTO
    {
        public List<ZListInfoDTO> ZLIST { get; set; }
        public List<int> ZLIST_D { get; set; }

        public RTLSZonesInfoDTO()
        {
            ZLIST = new List<ZListInfoDTO>();
            ZLIST_D = new List<int>();
        }
    }
}