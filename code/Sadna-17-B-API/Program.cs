using Microsoft.OpenApi.Models;
using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer;
using Sadna_17_B.ServiceLayer;
using Sadna_17_B.ServiceLayer.Services;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Web;
using System;
using Sadna_17_B.DomainLayer.User;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register your IUserService and its implementation UserService
//builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<ServiceFactory>();
//builder.Services.AddScoped<UserController>();
//builder.Services.AddScoped<OrderSystem>();
//builder.Services.AddScoped<StoreController>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sadna-17-B-API", Version = "v1" });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();
int count = 0;
NotificationSystem notificationsSystem = new NotificationSystem();
app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var username = context.Request.Query["username"];
        NotificationsHandler notificationsHandler = NotificationsHandler.getInstance();
        if (count == 0)
        {
            Task.Run(() =>
            {
                count++;
                Thread.Sleep(8000);
                int index = 0;
                while (true)
                {
                    notificationsSystem.Notify("noam", "important notification" + index);
                    Thread.Sleep(1000);
                    index++;
                }
            });

        }
        var ws = await context.WebSockets.AcceptWebSocketAsync();
        await notificationsHandler.addConnection(username, ws);
        Console.WriteLine("connected " + username);
    }

});


//app.Map("/ws", async context =>
//{
//    if (context.WebSockets.IsWebSocketRequest)
//    {
//        var curName = context.Request.Query["name"];

//        using var ws = await context.WebSockets.AcceptWebSocketAsync();

//        connections.Add(ws);

//        await Broadcast($"{curName} joined the room");
//        await Broadcast($"{connections.Count} users connected");
//        await ReceiveMessage(ws,
//            async (result, buffer) =>
//            {
//                if (result.MessageType == WebSocketMessageType.Text)
//                {
//                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
//                    await Broadcast(curName + ": " + message);
//                }
//                else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
//                {
//                    connections.Remove(ws);
//                    await Broadcast($"{curName} left the room");
//                    await Broadcast($"{connections.Count} users connected");
//                    await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
//                }
//            });
//    }
//    else
//    {
//        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//    }
//});
//async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
//{
//    var buffer = new byte[1024 * 4];
//    while (socket.State == WebSocketState.Open)
//    {
//        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
//        handleMessage(result, buffer);
//    }
//}

//async Task Broadcast(string message)
//{
//    var bytes = Encoding.UTF8.GetBytes(message);
//    foreach (var socket in connections)
//    {
//        if (socket.State == WebSocketState.Open)
//        {
//            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
//            await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
//        }
//    }
//}

app.Run();
