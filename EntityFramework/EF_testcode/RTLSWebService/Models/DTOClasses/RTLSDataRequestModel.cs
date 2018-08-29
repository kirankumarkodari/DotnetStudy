namespace RTLSWebService.Models.DTOClasses
{
    public class RTLSDataRequestModel
    {
        public string PID { get; set; }
        public string Password { get; set; }
        public string PrimaryIMEI { get; set; }
        public string DataVersion { get; set; }
        public string LastSyncDate { get; set; }
    }
}