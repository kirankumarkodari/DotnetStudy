using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace RTLSWebService.Common
{
    public class RTLSResponse
    {
        
        public string Version { get; set; }
        
        public int HTTPStatusCode { get; set; }
      
        public string HTTPErrorMessage { get; set; }
    }
}