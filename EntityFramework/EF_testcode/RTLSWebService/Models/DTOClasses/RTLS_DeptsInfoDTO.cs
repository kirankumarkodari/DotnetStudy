using RTLSWebService.Models.DTOClasses.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RTLSWebService.Models.DTOClasses
{
    public class RTLS_DeptsInfoDTO
    {
        public List<DeptInfoDTO> DEPT { get; set; }
        public List<int> DEPT_D { get; set; }

        public RTLS_DeptsInfoDTO()
        {
            DEPT = new List<DeptInfoDTO>();
            DEPT_D = new List<int>();
        }

    }
}