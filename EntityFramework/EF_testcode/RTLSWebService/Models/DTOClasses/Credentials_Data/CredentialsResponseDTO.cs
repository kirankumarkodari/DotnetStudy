using RTLSWebService.Common;

namespace RTLSWebService.Models.DTOClasses.Credentials_Data
{
    public class CredentialsResponseDTO : RTLSResponse
    {
        public CredentialsDATADTO DATA { get; set; }
    }
}