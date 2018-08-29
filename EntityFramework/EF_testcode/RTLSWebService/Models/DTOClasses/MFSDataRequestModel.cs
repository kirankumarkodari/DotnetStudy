using System.Collections.Generic;

namespace RTLSWebService.Models.DTOClasses
{

    public class MFSDataRequestModel
    {
        public string PID { get; set; }
        public string PASSWORD { get; set; }
        public string IMEI { get; set; }
        public string LastSyncDate { get; set; }
        public List<MFSDataReceivedInfoDTO> STATUS { get; set; }

        public MFSDataRequestModel()
        {
            STATUS = new List<MFSDataReceivedInfoDTO>();
        }

    }
}