using Newtonsoft.Json.Linq;
using RTLSWebService.Common;
using RTLSWebService.Models.DTOClasses;
using RTLSWebService.Models.DTOClasses.MFS_Data;
using RTLSWebService.Models.RTLS_DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace RTLSWebService.Controllers
{
    public class MakeFacilityStatusController : ApiController
    {

        private tata_rakshak db = new tata_rakshak();
        // POST: Services/MakeFacilityStatus
        DbContextTransaction rtlsDBTran = null;
        public IHttpActionResult Post([FromBody]JToken jsonData)
        {
            var response = new MFSResponse();
            response.Version = "1.0.0.0";
            response.HTTPErrorMessage = "";
            response.HTTPStatusCode = 200;
            var data = new MFSDataDTO();

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
                    var statusesInfo = jsonData.ToObject<MFSDataRequestModel>();
                    var userIsValid = CommonMethods.UserAuthurization(statusesInfo.PID, statusesInfo.PASSWORD);

                    if (userIsValid == true)
                    {
                        if (statusesInfo.STATUS != null)
                        {
                            rtlsDBTran = db.Database.BeginTransaction();
                            List<MFSResponseInfoDTO> statusResponses = new List<MFSResponseInfoDTO>();


                            foreach (var statusRecord in statusesInfo.STATUS)
                            {
                                MFSResponseInfoDTO statusResponse = new MFSResponseInfoDTO();
                                try
                                {
                                    RTLS_PERSONDTLS person = (from p in db.RTLS_PERSONDTLS
                                                              where p.PERSON_ID == statusRecord.P_ID
                                                              select p).FirstOrDefault();
                                    RTLS_PERSONSTATUS pStatus = (from p in db.RTLS_PERSONSTATUS
                                                                 where p.person_id == statusRecord.P_ID
                                                                 select p).FirstOrDefault();

                                    if (pStatus != null)
                                    {
                                        pStatus.zone_id = -1;
                                        pStatus.status = statusRecord.FSTATUS;
                                        pStatus.created_time = CommonMethods.getDateFromString(statusRecord.UDATE);
                                        pStatus.stime = CommonMethods.getDateFromString(statusRecord.SDATE);
                                    }
                                    RTLS_ONLINEPERSONSTATUS onlineStatus = (from p in db.RTLS_ONLINEPERSONSTATUS
                                                                            where p.PERSONID == statusRecord.P_ID
                                                                            select p).FirstOrDefault();
                                    if (onlineStatus != null)
                                    {

                                        onlineStatus.Zone_ID = -1;
                                        onlineStatus.status = statusRecord.FSTATUS;
                                        onlineStatus.created_time = CommonMethods.getDateFromString(statusRecord.UDATE);
                                    }

                                    //tag  Un-assignment
                                    if (statusRecord.T_STATUS == (int)Constants.Tag.UnAssignment)
                                    {
                                        if (person != null)//Update PersonDetails Table with current by making tagId = -1 with Unassigned time as current time.
                                        {
                                            person.TAG_ID = -1;
                                            person.Un_Assigned_time = DateTime.Now;
                                            person.CREATED_TIME = DateTime.Now;
                                            person.Tagtransact_type = 0;
                                            db.Entry(person).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        //Update RTLS_PERSONSTATUS Table
                                        if (pStatus != null)
                                        {
                                            pStatus.tagid = -1;
                                            pStatus.created_time = DateTime.Now;
                                            db.Entry(pStatus).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }

                                        //Update RTLS_ONLINEPERSONSTATUS Table
                                        if (onlineStatus != null)
                                        {
                                            onlineStatus.TAGID = -1;
                                            onlineStatus.created_time = DateTime.Now;
                                            db.Entry(onlineStatus).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                    else if (statusRecord.T_STATUS == (int)Constants.Tag.Assignment) ////tag assignment
                                    {
                                        if (person != null) // Insert Current tag id in PersonDetails table with ASSignedTime as current time              
                                        {
                                            person.TAG_ID = statusRecord.TAGID;
                                            person.Assigned_time = DateTime.Now;
                                            person.Tagtransact_type = 1;
                                            person.ACTINACT = 1;
                                        }
                                        //Update RTLS_PERSONSTATUS Table
                                        if (pStatus != null)
                                        {

                                            pStatus.tagid = statusRecord.TAGID;
                                            db.Entry(pStatus).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        //Update RTLS_ONLINEPERSONSTATUS Table
                                        if (onlineStatus != null)
                                        {
                                            onlineStatus.TAGID = statusRecord.TAGID;
                                            db.Entry(onlineStatus).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }

                                    }
                                    else //Do nothing
                                    { }
                                    if (statusRecord.FSTATUS != 0)
                                    {
                                        //Update RTLS_PERSONSTATUS Table
                                        if (pStatus != null)
                                        {
                                            db.Entry(pStatus).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }

                                        //Insert into RTLS_PERSONSTATUS_HISTORY
                                        var status = new RTLS_PERSONSTATUS_HISTORY();
                                        status.person_id = statusRecord.P_ID;
                                        if (statusRecord.T_STATUS == (int)Constants.Tag.UnAssignment)
                                        {
                                            status.tagid = -1;
                                        }
                                        else
                                        {
                                            status.tagid = statusRecord.TAGID;

                                        }
                                        status.zone_id = -1;
                                        status.status = statusRecord.FSTATUS;
                                        status.created_time = CommonMethods.getDateFromString(statusRecord.UDATE);
                                        status.stime = CommonMethods.getDateFromString(statusRecord.SDATE);
                                        db.Entry(status).State = EntityState.Added;
                                        db.SaveChanges();

                                    }
                                    statusResponse.SID = statusRecord.S_ID;
                                    statusResponse.Saved = (int)Constants.StatusSaved.Yes;
                                    statusResponse.ERROR = "";
                                    statusResponses.Add(statusResponse);

                                }
                                catch (Exception e)
                                {
                                    //frame response formate 
                                    statusResponse.SID = statusRecord.S_ID;
                                    statusResponse.Saved = (int)Constants.StatusSaved.No;
                                    statusResponse.ERROR = e.Message;
                                    statusResponses.Add(statusResponse);

                                }
                            }
                            rtlsDBTran.Commit();
                            MFSARInfoDTO ar = new MFSARInfoDTO();
                            ar.StatusResponses = statusResponses;
                            data.ActionResult = ar;
                            data.ResponseCode = (int)Constants.DataRC.StatusPostedSuccessfully;
                            data.ErrorMessage = "";
                            response.DATA = data;

                            return Ok(response);
                        }
                        else
                        {
                            emptyDataInfo.ResponseCode = (int)Constants.DataRC.RequestBodyIsEmpty;
                            emptyDataInfo.ErrorMessage = "There is no statuses to save";
                            emptyDataInfo.ActionResult = emptyAr;
                            emptyDataResponse.DATA = emptyDataInfo;
                            return Ok(emptyDataResponse);
                        }
                    }
                    else
                    {
                        //return response as unauthorized user
                        emptyDataInfo.ResponseCode = (int)Constants.DataRC.InvalidCredentials;
                        emptyDataInfo.ErrorMessage = "Invalid User Name and Password";
                        emptyDataInfo.ActionResult = emptyAr;
                        emptyDataResponse.DATA = emptyDataInfo;
                        return Ok(emptyDataResponse);
                    }
                }
            }
            catch (Exception saveExp)
            {
                if (rtlsDBTran != null)
                {
                    rtlsDBTran.Rollback();
                }
                emptyDataInfo.ResponseCode = (int)Constants.DataRC.InternalException;
                emptyDataInfo.ErrorMessage = saveExp.Message;
                emptyDataInfo.ActionResult = emptyAr;
                emptyDataResponse.DATA = emptyDataInfo;
                return Ok(emptyDataResponse);
            }
        }
    }
}
