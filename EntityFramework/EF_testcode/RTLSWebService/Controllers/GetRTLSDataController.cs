using Newtonsoft.Json.Linq;
using RTLSWebService.Common;
using RTLSWebService.Models.DTOClasses;
using RTLSWebService.Models.DTOClasses.MasterData;
using RTLSWebService.Models.DTOClasses.OnlineData;
using RTLSWebService.Models.RTLS_DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
namespace RTLSWebService.Controllers
{

    public class GetRTLSDataController : ApiController
    {

        private tata_rakshak db = new tata_rakshak();


        public async Task<IHttpActionResult> Post([FromBody]JObject rtlsDataInfo)
        {

            var response = new GetRTLSDataDTO();
            response.Version = "1.0.0.0";
            response.HTTPErrorMessage = "";
            response.HTTPStatusCode = 200;
            var data = new RTLSDataInfoDTO();
            var actionResultData = new RTLSDataARInfoDTO();


            var emptyDataResponse = new EmptyDataResponse();
            emptyDataResponse.Version = "1.0.0.0";
            emptyDataResponse.HTTPErrorMessage = "";
            emptyDataResponse.HTTPStatusCode = 200;
            var emptyDataInfo = new EmptyDataInfo();
            var emptyAr = new EmptyARResponse();

            if (rtlsDataInfo == null)
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
                dynamic json = rtlsDataInfo;

                string lastSyncDateAtClient = json.LastSyncDate;
                if (lastSyncDateAtClient != null)
                {

                    try
                    {
                        byte[] emptyByteArray = new byte[0];
                        int[] emptyLocationArray = new int[0];
                        var convertedLocationArray = emptyLocationArray.ToList();
                        var lastSyncDateAtServer = "";
                        var lsdAts = DateTime.Now.AddDays(-100);

                        var ls = db.RTLS_SYNC_DTLS.Select(d => d.last_sync_time).ToList();
                        foreach (var item in ls)
                        {
                            lastSyncDateAtServer = item.ToString("dd-MM-yyyy HH:mm:ss");
                            lsdAts = item;
                        }

                        var lsyncAtc = CommonMethods.getDateFromString(lastSyncDateAtClient);
                        var lsdAtc = CommonMethods.StartOfDay(lsyncAtc);
                        int result = DateTime.Compare(lsdAts, lsdAtc);

                        var todayEndDate = CommonMethods.endOfDay(DateTime.Now);
                        if (result > 0) //lastSyncDateAtServer is later than lastSyncDateAtClient
                        {//last sync date is less than creation date  

                            //Get DEPTS
                            var departments = new RTLS_DeptsInfoDTO();

                            var depts = await (from d in db.RTLS_DEPARTMENTS
                                               where (lsyncAtc <= d.created_time && d.created_time <= todayEndDate)
                                               where d.opeartion_type == 1
                                               select new DeptInfoDTO { DPNO = (int)d.dept_id, DPNAME = d.dept_name }).AsNoTracking().ToListAsync();
                            var depts_d = await ((db.RTLS_DEPARTMENTS.Where(p => p.opeartion_type != 1 && (lsyncAtc <= p.created_time && p.created_time <= todayEndDate))).Select(q => (int)q.dept_id)).ToListAsync();

                            departments.DEPT = depts;
                            departments.DEPT_D = depts_d;

                            //Get DESGS
                            var designations = new RTLSDesgsInfoDTO();
                            var desigs = await (from d in db.RTLS_DESIGNATIONS
                                                where (lsyncAtc <= d.created_time && d.created_time <= todayEndDate)
                                                where d.opeartion_type == 1
                                                select new DesgInfoDTO { DNO = (int)d.desg_id, DNAME = d.desg_name }).AsNoTracking().ToListAsync();
                            var desigs_d = await ((db.RTLS_DESIGNATIONS.Where(p => p.opeartion_type != 1 && (lsyncAtc <= p.created_time && p.created_time <= todayEndDate))).Select(q => (int)q.desg_id)).ToListAsync();

                            designations.DESG = desigs;
                            designations.DESG_D = desigs_d;

                            //Get PERSON
                            var persons = new RTLSPersonsInfoDTO();


                            var pList = (from item in (from p in db.RTLS_PERSONDTLS
                                                       where p.OPERATION_TYPE == 1
                                                       where (lsyncAtc <= p.CREATED_TIME && p.CREATED_TIME <= todayEndDate)

                                                       let pPhotoRow = (from q in db.Cloud_persons_images
                                                                        where q.img_name == p.PERSON_ID
                                                                        where (lsyncAtc <= q.Createdtime && q.Createdtime <= todayEndDate)
                                                                        select q).FirstOrDefault()
                                                       select new
                                                       {
                                                           p,
                                                           Img_ext = (pPhotoRow.Img_ext ?? string.Empty),
                                                           photoBytes = pPhotoRow.Person_img ?? emptyByteArray
                                                       }).ToList()

                                         select new PersonListInfoDTO
                                         {
                                             P_ID = item.p.PERSON_ID,
                                             T_ID = (int)item.p.TAG_ID.Value,
                                             PNAME = item.p.PERSONNAME,
                                             DEPT = (int)item.p.DEPARTMENT_id,
                                             DESG = (int)item.p.DESIGNATION_id,
                                             MOB_NO = item.p.MOBILE_NO,
                                             ACTINACT = (int)item.p.ACTINACT,
                                             PHOTO = new PersonPhotoInfo { PDATA = Convert.ToBase64String(item.photoBytes), PEXT = item.Img_ext },
                                             P_ZONES = (from zone in db.RTLS_TAG_ZONE_CONFIG
                                                        where (lsyncAtc <= zone.created_time && zone.created_time <= todayEndDate)
                                                        where zone.tagid == item.p.TAG_ID
                                                        select (int)zone.zone_id).ToList()
                                         }).ToList();


                            var pList_d = await ((db.RTLS_PERSONDTLS.Where(p => p.OPERATION_TYPE != 1 && (lsyncAtc <= p.CREATED_TIME && p.CREATED_TIME <= todayEndDate))).Select(q => q.PERSON_ID)).AsNoTracking().ToListAsync();

                            persons.PLIST = pList;
                            persons.PLIST_D = pList_d;

                            //Get SMS
                            var sms = await (from s in db.RTLS_SMS_TRANSACTIONS
                                             where (lsyncAtc <= s.DELIVERED_TIME && s.DELIVERED_TIME <= todayEndDate)

                                             select new SmsInfoDTO
                                             {
                                                 SMS_ID = s.SMS_ID,
                                                 P_ID = s.person_id,
                                                 T_ID = (int)s.tagid,
                                                 F_NO = (int)s.fltno,
                                                 MSG = s.MSG,
                                                 MOBILE_NO = s.REC_MOBILENO,
                                                 SENT_NOT = (int)s.sent_notsent,
                                                 S_TIME = (s.SENT_TIME).ToString(),
                                                 IS_DELV = s.DELIVERY_STATUS,
                                                 DELV_TIME = (s.DELIVERED_TIME).ToString()
                                             }).AsNoTracking().ToListAsync();

                            //Get ZONES
                            var zones = new RTLSZonesInfoDTO();
                            var zlist = await (from z in db.RTLS_ZONEDTLS
                                               where z.oparation_type == 1
                                               where (lsyncAtc <= z.created_time && z.created_time <= todayEndDate)
                                               select new ZListInfoDTO { Z_ID = (int)z.zone_id, GID = (int)z.gid, LOC = z.zone_name, AREA = z.area, S_US = (int)z.safe_unsafe }).AsNoTracking().ToListAsync();

                            var zlist_d = await ((db.RTLS_ZONEDTLS.Where(z => z.oparation_type != 1 && (lsyncAtc <= z.created_time && z.created_time <= todayEndDate))).Select(q => (int)q.zone_id)).ToListAsync();

                            zones.ZLIST = zlist;
                            zones.ZLIST_D = zlist_d;

                            //Get FAULTS
                            var faults = await (from f in db.RTLS_PERTRKFAULTINFO
                                                where (f.CREATEDTIME >= lsyncAtc && f.CREATEDTIME <= todayEndDate)
                                                select new FaultInfoDTO { F_NO = (int)f.FLTNO, F_NAME = f.fltname, F_DESC = f.FLTDESC }).AsNoTracking().ToListAsync();
                            //RTLS_Constants
                            var maxTags = await db.RTLS_PERTRK_CONFIG.Select(t => t.MaxTAGIDNum).FirstOrDefaultAsync();
                            var rtlsConstants = new RTLSConstantsInfoDTO();
                            rtlsConstants.MAX_TAGS = (int)maxTags;

                            //RTLS_LevelZones 

                            /* var leveZones =  (from s in db.RTLS_SpotLevelMap
                             group s by s.Level_no
                             into g
                             select new RTLSLevelZonesInfoDTO
                             {
                                 L1 = (g.Key == 1) ? (List<int>)g: convertedLocationArray,
                                 L2 = (g.Key == 2) ? (List<int>)g : convertedLocationArray,
                                 L3 = (g.Key == 3) ? (List<int>)g : convertedLocationArray

                             }).FirstOrDefault(); */
                            /* var lookup = db.RTLS_SpotLevelMap.ToLookup(s => s.Level_no, s => s.zone_id);
                             var levels = new RTLSLevelZonesInfoDTO
                             {
                                 L1 = lookup[1].ToList(),
                                 L2 = lookup[2].ToList(),
                                 L3 = lookup[3].ToList()
                             };*/
                            var levelZones = new RTLSLevelZonesInfoDTO();

                            var l1 = await ((db.RTLS_SpotLevelMap.Where(p => p.Level_no == 1)).Select(q => (int)q.zone_id)).ToListAsync();
                            var l2 = await ((db.RTLS_SpotLevelMap.Where(p => p.Level_no == 2)).Select(q => (int)q.zone_id)).ToListAsync();
                            var l3 = await ((db.RTLS_SpotLevelMap.Where(p => p.Level_no == 3)).Select(q => (int)q.zone_id)).ToListAsync();

                            levelZones.L1 = l1;
                            levelZones.L2 = l2;
                            levelZones.L3 = l3;


                            //a) Maste Data preparation
                            var masterData = new MasterDataInfoDTO()
                            {
                                DP_LIST = departments,
                                DS_LIST = designations,
                                SMS = sms,
                                FAULTS = faults,
                                PERSONS = persons,
                                ZONES = zones,
                                RTLS_CNTS = rtlsConstants,
                                LEVEL_ZONES = levelZones
                            };

                            //b) ONLINE_DATA Preparation
                            DateTime startOfThisDay = CommonMethods.StartOfDay(DateTime.Now);

                            var peopleStatus = (from od in db.RTLS_ONLINEPERSONSTATUS
                                                where (lsdAtc <= od.created_time && od.created_time <= todayEndDate)
                                                let zoneIds = db.RTLS_PERSONSTATUS_HISTORY.Where(p => p.person_id == od.PERSONID)
                                                .OrderByDescending(z => z.stime >= startOfThisDay && z.stime <= todayEndDate)
                                                .Select(z => z.zone_id)
                                                .ToList()
                                                select new
                                                {
                                                    Person = od,
                                                    ZoneIds = zoneIds,
                                                });


                            var onlineData = (from od in peopleStatus


                                              let location = (from zone in db.RTLS_ZONEDTLS
                                                              where zone.zone_id == od.Person.Zone_ID
                                                              select zone.area).FirstOrDefault()

                                              // let zoneIdsArray = getZoneList((od.ZoneIds.ToArray()))
                                              //  let fzones = zoneIdsArray.Select(z => z).Take(4)




                                              select new OnlineDataInfoDTO
                                              {
                                                  P_ID = od.Person.PERSONID,
                                                  T_ID = (int)od.Person.TAGID,
                                                  Z_ID = (od.Person.created_time >= startOfThisDay) ? (int)od.Person.Zone_ID : -1,
                                                  LOC = (location != null ? location : " "),
                                                  STATUS = (od.Person.created_time >= startOfThisDay) ? (int)od.Person.status : 6,
                                                  T_BAT_SIG_STR = (int)od.Person.TAG_SIGNALSTRENGTH,
                                                  B_BAT_SIG_STR = (int)od.Person.BS_SIGNALSTRENGTH,
                                                  T_BAT_STA_VAL = (int)od.Person.TAG_BATTERY_STATUS_VAL,
                                                  T_BAT_STA_PERCNT = (int)od.Person.TAG_BATTERY_STATUS_PERCNT,
                                                  BS_BAT_STA_VAL = (int)od.Person.BS_BATTERY_STATUS_VAL,
                                                  BS_BAT_STA_PER = (int)od.Person.BS_BATTERY_STATUS_PERCNT,
                                                  IN_TIME = (od.Person.INTIME).ToString(),
                                                  ALL_NOT_TME = (od.Person.ALLISNOTWELLTIME).ToString(),
                                                  P_TME = (od.Person.PANICTIME).ToString(),
                                                  NO_M_TME = (od.Person.NOMOTIONTIME).ToString(),
                                                  OUT_TME = (od.Person.OUT_TIME).ToString(),
                                                  TEMP_TME = (od.Person.TEMPEXCEEDTIME).ToString(),
                                                  LOW_BAT_TME = (od.Person.LOW_BATTERY_TIME).ToString(),
                                                  FOUT_TME = (od.Person.FOUT_TIME).ToString(),
                                                  LAST_UPDATE_TIME = (od.Person.LASTUPDATEDTIME).ToString(),
                                                  TEMP_VAL = (decimal)(od.Person.TEMP_VALUE),

                                                  //  NO_OF_OUT = fOut,

                                                  NO_OF_OUT = (from o in db.RTLS_FAULT_DTLS
                                                               where (o.faultno == (int)Constants.Faults.LowBattery)
                                                               where (startOfThisDay <= o.ORC_DATETIME && o.ORC_DATETIME <= todayEndDate)
                                                               where (o.PERSON_ID.ToLower() == od.Person.PERSONID.ToLower())
                                                               select o.fltname).Count(),
                                                  NO_OF_PANIC = (from o in db.RTLS_FAULT_DTLS
                                                                 where o.faultno == (int)Constants.Faults.Panic
                                                                 where (startOfThisDay <= o.ORC_DATETIME && o.ORC_DATETIME <= todayEndDate)
                                                                 where (o.PERSON_ID.ToLower() == od.Person.PERSONID.ToLower())
                                                                 select o.fltname).Count(),
                                                  NO_OF_IN_ACTIVE = (from o in db.RTLS_FAULT_DTLS
                                                                     where o.faultno == (int)Constants.Faults.InActive
                                                                     where (startOfThisDay <= o.ORC_DATETIME && o.ORC_DATETIME <= todayEndDate)
                                                                     where (o.PERSON_ID == od.Person.PERSONID)
                                                                     select o.fltname).Count(),
                                                  NO_OF_TEMP = (from o in db.RTLS_FAULT_DTLS
                                                                where (o.faultno == (int)Constants.Faults.HighTemp || o.faultno == (int)Constants.Faults.LowTemp)
                                                                where (startOfThisDay <= o.ORC_DATETIME && o.ORC_DATETIME <= todayEndDate)
                                                                where (o.PERSON_ID == od.Person.PERSONID)
                                                                select o.fltname).Count(),
                                                  NO_OF_LBAT = (from o in db.RTLS_FAULT_DTLS
                                                                where o.faultno == (int)Constants.Faults.LowBattery
                                                                where (startOfThisDay <= o.ORC_DATETIME && o.ORC_DATETIME <= todayEndDate)
                                                                where (o.PERSON_ID == od.Person.PERSONID)
                                                                select o.fltname).Count(),

                                                  LOCS = convertedLocationArray


                                              }).ToList();

                            if (onlineData.Count == 0)
                            {

                                actionResultData.LastSyncDate = lastSyncDateAtServer;
                                actionResultData.ONLINE_DATA = onlineData;
                                actionResultData.M_DATA = masterData;

                                data.ResponseCode = (int)Constants.DataRC.NoOnlineDataAvailable;
                                data.ErrorMessage = "NoOnlineDataAvailable";
                                data.ActionResult = actionResultData;

                                response.DATA = data;
                                return Ok(response);
                            }
                            else
                            {
                                actionResultData.LastSyncDate = lastSyncDateAtServer;
                                actionResultData.ONLINE_DATA = onlineData;
                                actionResultData.M_DATA = masterData;

                                data.ResponseCode = (int)Constants.DataRC.Success;
                                data.ErrorMessage = "";
                                data.ActionResult = actionResultData;

                                response.DATA = data;
                                return Ok(response);

                            }

                        }
                        else
                        {
                            emptyDataInfo.ResponseCode = (int)Constants.DataRC.NoChangeInMasterData;
                            emptyDataInfo.ErrorMessage = "No Change in MasterData";
                            emptyAr.LastSyncDate = lastSyncDateAtServer;
                            emptyDataInfo.ActionResult = emptyAr;
                            emptyDataResponse.DATA = emptyDataInfo;
                            return Ok(emptyDataResponse);
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
                else
                {
                    emptyDataInfo.ResponseCode = (int)Constants.DataRC.RequestBodyIsEmpty;
                    emptyDataInfo.ErrorMessage = "Last sync date found as null in requesting url body";
                    emptyDataInfo.ActionResult = emptyAr;
                    emptyDataResponse.DATA = emptyDataInfo;
                    return Ok(emptyDataResponse);

                }

            }
        }
        public int[] getZoneList(decimal[] zoneIdsArray)
        {
            int[] zoneIds = Array.ConvertAll(zoneIdsArray, x => (int)x);
            List<int> list = zoneIds.ToList();
            for (int c = 1; c < zoneIdsArray.Count(); c++)
            {
                if (zoneIdsArray[c] == zoneIdsArray[c - 1])
                {
                    list.Remove((int)zoneIdsArray[c]);
                }
            }
            return list.ToArray();
        }

    }


}
