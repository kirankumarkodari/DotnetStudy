using RTLSWebService.Models.DTOClasses.OnlineData;
using System.Collections.Generic;

namespace RTLSWebService.Models.DTOClasses
{
    public class RTLSDataARInfoDTO
    {

        public string LastSyncDate { get; set; }
        public List<OnlineDataInfoDTO> ONLINE_DATA { get; set; }
        public MasterDataInfoDTO M_DATA { get; set; }

        public RTLSDataARInfoDTO()
        {
            ONLINE_DATA = new List<OnlineDataInfoDTO>();
        }


    }
}