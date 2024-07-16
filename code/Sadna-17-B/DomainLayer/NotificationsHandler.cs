using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web.WebSockets;
using System.Net;
using System.Text;
using Sadna_17_B.DomainLayer.User;
using System.Collections.Generic;
using Sadna_17_B.Utils;
using System.Text.Json;


namespace Sadna_17_B.DomainLayer
{
    public class NotificationsHandler
    {
        private static ConcurrentDictionary<string, WebSocket> userSockets = new ConcurrentDictionary<string, WebSocket>();
        private static NotificationsHandler Instance = null;
        private Dictionary<string, List<Notification>> notifications; // username -> List<Notification>
        private NotificationsHandler() {
            notifications = new Dictionary<string, List<Notification>>();
        }
        public static NotificationsHandler getInstance()
        {
            if (Instance == null)
                Instance = new NotificationsHandler();
            return Instance;
        }

        public async Task addConnection(string username, WebSocket ws)
        {
            //if (userSockets.TryGetValue(username, out WebSocket socket))
            //{
            //    await socket.CloseAsync();
            //}
            userSockets[username] = ws;
            NotificationSystem.getInstance().NotifyLogin(username);
            Console.WriteLine("added socket " + username);
            await processConnection(ws);
            
        }

        public async Task processConnection(WebSocket ws)
        {
            try
            {
                var buffer = new byte[1024 * 4];
                while (true)
                {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer),
                CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                }
            }
            catch (Exception ignored) { Console.WriteLine("error"); }
        }

        public async void Notify(string usernameToNotify, Notification notification)
        {
            try
            {
                Console.WriteLine($"{usernameToNotify} message ########");
                //Notification notification = new Notification(message);
                //if (!notifications.ContainsKey(usernameToNotify))
                //{
                //    notifications[usernameToNotify] = new List<Notification>();
                //}
                //notifications[usernameToNotify].Add(notification);
                string serialized = JsonSerializer.Serialize(notification);
                var bytes = Encoding.UTF8.GetBytes(serialized);
                var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                await userSockets[usernameToNotify].SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine("DONE");
            }
            catch (Exception ex)
            {
                userSockets.TryRemove(usernameToNotify, out WebSocket socket);
            }
        }

        public static async Task SendNotificationAsync(string username, string message)
        {
            //if (userSockets.TryGetValue(username, out WebSocket socket) && socket.State == WebSocketState.Open)
            //{
            //    var buffer = Encoding.UTF8.GetBytes(message);
            //    var segment = new ArraySegment<byte>(buffer);
            //    await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            //}
            //else if (socket.State != WebSocketState.Open)
            //{
            //    userSockets.TryRemove(username, out _);
            //}
        }
    }
}
