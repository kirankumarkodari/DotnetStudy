using RTLSWebService.Models.DTOClasses.MasterData;
using System.Collections.Generic;

namespace RTLSWebService.Models.DTOClasses
{
    public class RTLSDesgsInfoDTO
    {
        public List<DesgInfoDTO> DESG { get; set; }
        public List<int> DESG_D { get; set; }

        public RTLSDesgsInfoDTO()
        {
            DESG = new List<DesgInfoDTO>();
            DESG_D = new List<int>();
        }

    }
}