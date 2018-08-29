
using RTLSWebService.Models.DTOClasses.MasterData;
using System.Collections.Generic;

namespace RTLSWebService.Models.DTOClasses
{
    public class MasterDataInfoDTO
    {
        public RTLS_DeptsInfoDTO DP_LIST { get; set; }
        public RTLSDesgsInfoDTO DS_LIST { get; set; }
        public List<SmsInfoDTO> SMS { get; set; }
        public List<FaultInfoDTO> FAULTS { get; set; }
        public RTLSPersonsInfoDTO PERSONS { get; set; }
        public RTLSZonesInfoDTO ZONES { get; set; }
        public RTLSConstantsInfoDTO RTLS_CNTS { get; set; }
        public RTLSLevelZonesInfoDTO LEVEL_ZONES { get; set; }

        public MasterDataInfoDTO()
        {

            SMS = new List<SmsInfoDTO>();

            FAULTS = new List<FaultInfoDTO>();

        }

    }
}