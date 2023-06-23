using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Text;

namespace WebSocketApp.App_Source
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class wsFunction
    {
        private readonly IConnectionMultiplexer _redis;
        public wsFunction(/*IConnectionMultiplexer redis*/)
        {
            //_redis = redis;
        }
        public async Task ListenAcceptAsync(HttpContext context)
        {
            try
            {
                WebSocket ws = await context.WebSockets.AcceptWebSocketAsync();
                // Authenticate the WebSocket connection in Query
                var tokenQuery = context.Request.Query["token"].ToString();
                // Authenticate the WebSocket connection in Headers
                var tokenHeaders = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (!ValidateToken(tokenQuery))
                {
                    await ws.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Invalid token", CancellationToken.None);                    
                    return;
                }
                string Uname = context.Request.Query["name"].ToString();
                UserWebSocket uws = new UserWebSocket();
                uws.UserName = Uname;
                uws.UserWS = ws;
                UserList.ListDic.Add(Uname, uws);
                await ReciveAsync(uws);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task ReciveAsync(UserWebSocket uws)
        {
            try
            {
                WebSocket ws = uws.UserWS;
                string UserName = uws.UserName;
                byte[] buff;
                while (ws.State == WebSocketState.Open)
                {
                    buff = new byte[wsConfig.BufferSize];
                    WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>
                        (array: buff, offset: 0, count: buff.Length), CancellationToken.None);

                    if (result != null)
                    {
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            string strMsg = Encoding.UTF8.GetString(buff);
                            ReciveMessage? reciveMessage = JsonConvert.DeserializeObject<ReciveMessage>(strMsg);

                            UserWebSocket reciveUWS;
                            if (reciveMessage != null)
                            {
                                reciveUWS = UserList.ListDic[reciveMessage.Reciver];

                                SenderMessage SendMsg = new SenderMessage()
                                {
                                    Message = reciveMessage.Message,
                                    Sender = UserName
                                };
                                await SendAsync(reciveUWS.UserWS, SendMsg);
                            }
                        }

                        if (result.MessageType == WebSocketMessageType.Binary)
                        {
                            while (!result.EndOfMessage)
                            {
                                if (File.Exists("NewPic.png"))
                                {
                                    byte[] fileUp = File.ReadAllBytes("NewPic.png");
                                    byte[] NewBuff = new byte[fileUp.Length + result.Count];
                                    Buffer.BlockCopy(fileUp, 0, NewBuff, 0, fileUp.Length);
                                    Buffer.BlockCopy(buff, 0, NewBuff, fileUp.Length, result.Count);
                                    File.WriteAllBytes("NewPic.png", NewBuff);
                                }
                                else
                                {
                                    byte[] UpFile = new byte[result.Count];
                                    Buffer.BlockCopy(buff, 0, UpFile, 0, result.Count);
                                    File.WriteAllBytes("NewPic.png", UpFile);
                                }
                                if (result.EndOfMessage)
                                    await SendAsync(ws, "فایل ارسال شد");
                            }
                        }
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await ws.CloseAsync(WebSocketCloseStatus.Empty, "Closed websocket", CancellationToken.None);
                            UserList.ListDic.Remove(UserName);
                            return;
                        }
                    }
                }
            }
            catch (Exception)
            {
                await uws.UserWS.CloseAsync(WebSocketCloseStatus.Empty, "Closed websocket", CancellationToken.None);
                throw;
            }
        }

        public async Task SendAsync(WebSocket ws, string StrMsg)
        {
            byte[] buff = Encoding.UTF8.GetBytes(StrMsg);
            await ws.SendAsync(buff, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendAsync(WebSocket webSocket, SenderMessage senderMessage)
        {
            string StrMsg = JsonConvert.SerializeObject(senderMessage);
            byte[] buff = Encoding.UTF8.GetBytes(StrMsg);
            await webSocket.SendAsync(new ArraySegment<byte>
                (array: buff, offset: 0, count: buff.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        
        private bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(wsConfig.SecretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = wsConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = wsConfig.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
