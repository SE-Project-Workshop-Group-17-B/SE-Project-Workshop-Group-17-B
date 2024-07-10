using Microsoft.OpenApi.Models;
using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using Sadna_17_B.ServiceLayer.Services; 
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register your IUserService and its implementation UserService
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserController>();
builder.Services.AddScoped<OrderSystem>();
builder.Services.AddScoped<StoreController>();


//builder.Services.AddScoped<UserController>(); // Register UserController
//builder.Services.AddScoped<OrderSystem>();
//builder.Services.AddScoped<Order>();
//builder.Services.AddScoped<SubOrder>();
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("mydatabase"));
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

app.Run();
