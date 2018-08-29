using Microsoft.Azure.NotificationHubs;

namespace WebServiceRest
{
    public class Notifications
    {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://rtlsnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=zmoSvRKjhKkX361Z8AoaiRXwqHZGiKln0TGF6bjs/wM=",
                                                                         "RTLSHub");
        }
    }
}