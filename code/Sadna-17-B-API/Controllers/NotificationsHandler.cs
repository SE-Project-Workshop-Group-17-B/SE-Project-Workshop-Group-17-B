using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Sadna_17_B_API.Controllers
{
    public class NotificationsHandler
    {
        private ConcurrentDictionary<string, Tuple<HttpContext, WebSocket>> userSockets = new ConcurrentDictionary<string, Tuple<HttpContext, WebSocket>>();
        private static NotificationsHandler Instance = null;
        private List<WebSocket> connections = new List<WebSocket>();
        private NotificationsHandler() { }
        public static NotificationsHandler getInstance()
        {
            if (Instance == null)
                Instance = new NotificationsHandler();
            return Instance;
        }

        public async Task addConnection(string username, HttpContext context)
        {
            //if (userSockets.TryGetValue(username, out WebSocket socket))
            //{
            //    await socket.CloseAsync();
            //}
            //var ws = await context.WebSockets.AcceptWebSocketAsync();
            //userSockets[username] = new Tuple<HttpContext, WebSocket>(context, ws);
            //Console.WriteLine("added socket " + username);
            //var message = "The current time is: " + DateTime.Now.ToString("HH:mm:ss");
            //var bytes = Encoding.UTF8.GetBytes(message);
            //var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            //if (ws.State == WebSocketState.Open)
            //{
            //    await ws.SendAsync(arraySegment,
            //                        WebSocketMessageType.Text,
            //                        true,
            //                        CancellationToken.None);
            //    Console.WriteLine("message was sent");
            //}
            //connections.Add(ws);
        }


        //public static async Task SendNotificationAsync(string username, string message)
        //{
        //    if (userSockets.TryGetValue(username, out WebSocket socket) && socket.State == WebSocketState.Open)
        //    {
        //        var buffer = Encoding.UTF8.GetBytes(message);
        //        var segment = new ArraySegment<byte>(buffer);
        //        await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        //    }
        //    else if (socket.State != WebSocketState.Open)
        //    {
        //        userSockets.TryRemove(username, out _);
        //    }
        //}
    }
}
