namespace RTLSWebService.Models.DTOClasses.MasterData
{
    public class SmsInfoDTO
    {
        public string SMS_ID { get; set; }
        public string P_ID { get; set; }
        public int T_ID { get; set; }
        public int F_NO { get; set; }
        public string MSG { get; set; }
        public string MOBILE_NO { get; set; }

        public int SENT_NOT { get; set; }
        public string S_TIME { get; set; }
        public string IS_DELV { get; set; }
        public string DELV_TIME { get; set; }
    }
}