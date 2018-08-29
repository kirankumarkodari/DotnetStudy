using System.Collections.Generic;

namespace RTLSWebService.Models.DTOClasses.MFS_Data
{
    public class MFSARInfoDTO
    {
        public List<MFSResponseInfoDTO> StatusResponses { get; set; }

        public MFSARInfoDTO()
        {
            StatusResponses = new List<MFSResponseInfoDTO>();
        }
    }
}