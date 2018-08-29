using Microsoft.Azure.NotificationHubs;
using RTLSWebService.Common;
using RTLSWebService.Models.RTLS_DB;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebServiceRest;

namespace RTLSWebService.Controllers
{
    public class SendNotificationController : ApiController
    {
        private NotificationHubClient hub;
        private tata_rakshak db = new tata_rakshak();

        string oldNotificationString = WebApiApplication.previousNotificationString;

        // POST: Services/SendNotification
        public async Task<IHttpActionResult> Get()
        {

            var panicPersonsInfo = FrameNotificationString();
            if (panicPersonsInfo != "")
            {
                if (String.Compare(oldNotificationString, panicPersonsInfo) == 0)
                {
                    return Json("No Need to send same notification multiple times" + "Previous Notification: " + oldNotificationString);
                }
                else
                {
                    WebApiApplication.previousNotificationString = panicPersonsInfo;
                    var result = await broadcastNotification(panicPersonsInfo);
                    return Json(result);
                }
            }
            else
            {
                return Json("No need to send notification since there are no panic persons found");
            }

        }

        private async Task<string> broadcastNotification(string message)
        {

            hub = Notifications.Instance.Hub;

            try
            {
                NotificationOutcome outcome = await hub.SendGcmNativeNotificationAsync("{ \"data\" : {\"message\":\"" + message + "\" }}");

                var state = outcome.State;
                var notificationResult = outcome.Results;
                if (outcome != null)
                {
                    return "Notificatin Sent";
                }
                else
                {
                    return "probem with registration";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private string FrameNotificationString()
        {
            DateTime startOfThisDay = CommonMethods.StartOfDay(DateTime.Now);
            var panicPersonString = "";
            try
            {
                var panicPeoplesInfo = from od in db.RTLS_ONLINEPERSONSTATUS
                                       where (od.created_time >= startOfThisDay)
                                       where (od.status == (int)Constants.Status.Panic)
                                       select new { od.PERSONID, od.Zone_ID, od.PANICTIME, od.LOW_BATTERY_TIME, od.TEMPEXCEEDTIME };


                foreach (var item in panicPeoplesInfo)
                {
                    var currentPanicPersonInfo = "";
                    var personName = ((db.RTLS_PERSONDTLS.Where(p => p.PERSON_ID == item.PERSONID)).Select(q => q.PERSONNAME).SingleOrDefault());
                    var zoneName = ((db.RTLS_ZONEDTLS.Where(p => p.zone_id == item.Zone_ID)).Select(q => q.zone_name).SingleOrDefault());
                    var zoneSafety = ((db.RTLS_ZONEDTLS.Where(p => p.zone_id == item.Zone_ID)).Select(q => q.safe_unsafe).SingleOrDefault());
                    var msg = "";
                    var zoneSafeString = (zoneSafety == (int)Constants.zoneSafety.safeZone ? "safe zone" : "UnSafe zone");
                    var panicTime = ((DateTime)(item.PANICTIME)).ToString("ddd d MMM hh:mm:ss tt");
                    if (item.PANICTIME >= ((DateTime)(item.LOW_BATTERY_TIME)) && item.PANICTIME >= ((DateTime)(item.TEMPEXCEEDTIME)))
                    {
                        msg = "";
                        panicTime = ((DateTime)(item.PANICTIME)).ToString("ddd d MMM hh:mm:ss tt"); //("ddd d MMM");
                    }

                    if (((DateTime)(item.LOW_BATTERY_TIME)) >= item.PANICTIME && ((DateTime)(item.LOW_BATTERY_TIME)) >= item.TEMPEXCEEDTIME)
                    {
                        msg = "due to LOW BATTERY ";
                        panicTime = ((DateTime)(item.LOW_BATTERY_TIME)).ToString("ddd d MMM hh:mm:ss tt"); //("ddd d MMM");
                    }

                    if (((DateTime)(item.TEMPEXCEEDTIME)) >= item.PANICTIME && ((DateTime)(item.TEMPEXCEEDTIME)) >= item.LOW_BATTERY_TIME)
                    {
                        /* msg = "due to TEMPERATURE ";
                         panicTime = ((DateTime)(item.LOW_BATTERY_TIME)).ToString("ddd d MMM hh:mm:ss tt"); //("ddd d MMM");*/
                        return "";
                    }




                    currentPanicPersonInfo = personName + " [" + item.PERSONID + "] " + " is in PANIC at " + zoneName + " [" + zoneSafeString + "] " + msg + " since " + panicTime;

                    panicPersonString = panicPersonString + currentPanicPersonInfo + Environment.NewLine + Environment.NewLine;
                }

            }
            catch (Exception e)
            {
                return e.InnerException.Message;
            }

            return panicPersonString;

        }
    }
}
