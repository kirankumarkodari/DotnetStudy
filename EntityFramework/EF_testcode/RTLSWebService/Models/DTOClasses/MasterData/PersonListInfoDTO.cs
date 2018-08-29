using System.Collections.Generic;

namespace RTLSWebService.Models.DTOClasses.MasterData
{
    public class PersonListInfoDTO
    {
        public string P_ID { get; set; }
        public int T_ID { get; set; }
        public string PNAME { get; set; }
        public int DEPT { get; set; }
        public int DESG { get; set; }
        public string MOB_NO { get; set; }
        public int ACTINACT { get; set; }
        public PersonPhotoInfo PHOTO { get; set; }
        public List<int> P_ZONES { get; set; }

        public PersonListInfoDTO()
        {
            P_ZONES = new List<int>();

        }

    }
}