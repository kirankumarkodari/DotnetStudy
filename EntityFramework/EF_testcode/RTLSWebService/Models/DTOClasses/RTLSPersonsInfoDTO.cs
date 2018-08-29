using RTLSWebService.Models.DTOClasses.MasterData;
using System.Collections.Generic;

namespace RTLSWebService.Models.DTOClasses
{
    public class RTLSPersonsInfoDTO
    {
        public List<PersonListInfoDTO> PLIST { get; set; }
        public List<string> PLIST_D { get; set; }

        public RTLSPersonsInfoDTO()
        {
            PLIST = new List<PersonListInfoDTO>();
            PLIST_D = new List<string>();
        }

    }
}