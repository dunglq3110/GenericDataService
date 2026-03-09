using GenericDataService.GenericService;
using GenericDataService.GenericService.Notification;
using GenericDataService.Infrastructure;
using GenericDataService.Infrastructure.Pipeline;
using GenericDataService.Production.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFileOrigin", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // allows file:// origin in dev
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // required for SignalR
    });
});

//Custom services, check implementation to see all DI services registered
builder.Services.AddGenericService(builder.Configuration);

builder.Services.AddProductModule(builder.Configuration);


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
app.UseCors("AllowFileOrigin");
app.MapHub<EventHub>("/eventhub");
app.Run();
