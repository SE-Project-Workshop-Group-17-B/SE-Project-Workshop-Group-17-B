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
NotificationSystem notificationsSystem = NotificationSystem.getInstance();
app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var username = context.Request.Query["username"];
        NotificationsHandler notificationsHandler = NotificationsHandler.getInstance();
        //if (count == 0)
        //{
        //    count++;
        //    Task.Run(() =>
        //    {
               
        //        Thread.Sleep(8000);
        //        int index = 0;
        //        while (true)
        //        {
        //            notificationsSystem.Notify(username, "important notification" + index);
        //            Thread.Sleep(1000);
        //            index++;
        //        }
        //    });

        //}
        var ws = await context.WebSockets.AcceptWebSocketAsync();
        
        await notificationsHandler.addConnection(username, ws);
        Console.WriteLine("connected " + username);
    }

});

app.Run();
