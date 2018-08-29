using Newtonsoft.Json.Linq;
using RTLSWebService.Common;
using RTLSWebService.Models.DTOClasses.Credentials_Data;
using RTLSWebService.Models.RTLS_DB;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;

using System.Web.Http;

namespace RTLSWebService.Controllers
{
    public class CheckCredentialsController : ApiController
    {

        private tata_rakshak db = new tata_rakshak();
        private AES dataConv;
        // GET: Services/CheckCredentials

        public CheckCredentialsController()
        {
            dataConv = new AES();
        }
        public IHttpActionResult Post([FromBody]JObject jsonData)
        {
            var response = new CredentialsResponseDTO();
            response.Version = "1.0.0.0";
            response.HTTPErrorMessage = "";
            response.HTTPStatusCode = 200;

            var data = new CredentialsDATADTO();

            var emptyDataResponse = new EmptyDataResponse();
            emptyDataResponse.Version = "1.0.0.0";
            emptyDataResponse.HTTPErrorMessage = "";
            emptyDataResponse.HTTPStatusCode = 200;
            var emptyDataInfo = new EmptyDataInfo();
            var emptyAr = new EmptyARResponse();

            try
            {
                if (jsonData == null)
                {
                    emptyDataInfo.ResponseCode = (int)Constants.DataRC.RequestBodyIsEmpty;
                    emptyDataInfo.ErrorMessage = "";
                    emptyDataInfo.ActionResult = emptyAr;

                    emptyDataResponse.DATA = emptyDataInfo;
                    emptyDataResponse.HTTPErrorMessage = "Requesting url body is empty";
                    emptyDataResponse.HTTPStatusCode = (int)HttpStatusCode.BadRequest;
                    return Ok(emptyDataResponse);
                }
                else
                {

                    var userInfo = jsonData.ToObject<CredentialsDataRequestDTO>();
                    CredentialsARInfoDTO credentialsAR = new CredentialsARInfoDTO();
                    if ((userInfo == null) || (userInfo.PASSWORD == null)
                   || (userInfo.PASSWORD.Length == 0))
                    {
                        data.ResponseCode = (int)Constants.DataRC.RequestBodyIsEmpty;
                        data.ErrorMessage = "Either Person ID or Password is not provided";
                        response.HTTPStatusCode = (int)HttpStatusCode.BadRequest;
                        response.DATA = data;
                        return Ok(response);
                    }

                    if (userInfo.PID == null)
                    {
                        userInfo.PID = "";
                    }

                    string decryptPwd = dataConv.DecryptFromBase64(userInfo.PASSWORD);

                    int recCnt = db.RTLS_LOGINS.Count(user => user.USER_ID == userInfo.PID && user.PASSWORD == decryptPwd);
                    if (recCnt > 0)
                    {
                        credentialsAR.SID = (int)(from person in db.RTLS_LOGINS
                                                  where person.USER_ID == userInfo.PID
                                                  select person.SID).FirstOrDefault();
                        credentialsAR.LoginType = (int)(from person in db.RTLS_LOGINS
                                                        where person.USER_ID == userInfo.PID
                                                        select person.LOG_TYPE).FirstOrDefault();
                        credentialsAR.IsActive = (int)(from person in db.RTLS_LOGINS
                                                       where person.USER_ID == userInfo.PID
                                                       select person.ACTINACT).FirstOrDefault();
                        credentialsAR.PID = userInfo.PID;

                        data.ResponseCode = (int)Constants.DataRC.LoginSuccess;
                        data.ErrorMessage = "";
                        data.ActionResult = credentialsAR;

                        response.HTTPStatusCode = (int)HttpStatusCode.OK;
                        response.DATA = data;
                        //Store imei number in database 
                        if (userInfo.IMEI != null)
                        {
                            int imeiC = db.RTLS_PERSONSIMEIDATA.Count(imeiData => imeiData.IMEI == userInfo.IMEI);
                            if (imeiC == 0) //if current imei number is not stored at database
                            {
                                var imeiRecord = new RTLS_PERSONSIMEIDATA();
                                imeiRecord.IMEI = userInfo.IMEI;
                                imeiRecord.created_time = DateTime.Now;
                                try
                                {
                                    db.Entry(imeiRecord).State = EntityState.Added;
                                    db.SaveChanges();
                                }
                                catch
                                {

                                }
                            }
                        }

                        return Ok(response);

                    }
                    else
                    {
                        int recCnt1 = db.RTLS_LOGINS.Count(user => user.USER_ID == userInfo.PID);
                        if (recCnt1 > 0)
                        {
                            emptyDataInfo.ResponseCode = (int)Constants.DataRC.WrongPassword;
                            emptyDataInfo.ErrorMessage = "Wrong Password";
                            emptyDataInfo.ActionResult = emptyAr;
                            emptyDataResponse.DATA = emptyDataInfo;
                            return Ok(emptyDataResponse);
                        }

                        //return response as unauthorized user
                        emptyDataInfo.ResponseCode = (int)Constants.DataRC.InvalidCredentials;
                        emptyDataInfo.ErrorMessage = "Invalid User Name and Password";
                        emptyDataInfo.ActionResult = emptyAr;
                        emptyDataResponse.DATA = emptyDataInfo;
                        return Ok(emptyDataResponse);

                    }
                }
            }
            catch (Exception e)
            {
                emptyDataInfo.ResponseCode = (int)Constants.DataRC.InternalException;
                emptyDataInfo.ErrorMessage = e.Message;
                emptyDataInfo.ActionResult = emptyAr;
                emptyDataResponse.DATA = emptyDataInfo;
                return Ok(emptyDataResponse);
            }
        }
    }
}
