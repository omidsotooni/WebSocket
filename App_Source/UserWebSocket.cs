using System.Net.WebSockets;

namespace WebSocketApp.App_Source
{
    public class UserWebSocket
    {
        public string UserName { get; set; }
        public WebSocket UserWS { get; set; }
    }
}
