namespace WebSocketApp.App_Source
{
    public class wsConfig
    {
        public static int BufferSize = 4 * 1024;
        public static string wschatPath = "/wschat";
        public static string SecretKey = "qwertyuiopasdfghjklzxcvbnm123456";
        public static string Issuer = "JWT WebSocket";
        public static string Audience = "https://localhost:7219";
        public static WebSocketOptions GetOptions()
        {
            WebSocketOptions wso = new WebSocketOptions()
            {
                ReceiveBufferSize = BufferSize,
                KeepAliveInterval = TimeSpan.FromSeconds(120)
            };
            return wso;
        }
    }
}
