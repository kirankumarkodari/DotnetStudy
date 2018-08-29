using RTLSWebService.Models.RTLS_DB;
using System;
using System.Globalization;
using System.Linq;

namespace RTLSWebService.Common
{
    public class CommonMethods
    {



        // POST: Services/GetRTLSData

        public static bool UserAuthurization(string userName, string pwd)
        {
            AES dataConvert = new AES();
            tata_rakshak db = new tata_rakshak();

            string decryptPwd = dataConvert.DecryptFromBase64(pwd);
            int recCnt = db.RTLS_LOGINS.Count(user => user.USER_ID == userName && user.PASSWORD == decryptPwd);
            if (recCnt == 0)
            {
                return false;
            }
            else
            { return true; }
            // return true;


        }

        public static DateTime getDateFromString(string dateString)
        {
            CultureInfo enUS = new CultureInfo("en-US");

            DateTime dateValue;

            if (DateTime.TryParseExact(dateString, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out dateValue)) { }
            return dateValue;


        }
        public static DateTime StartOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }
        public static DateTime endOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }



    }

}