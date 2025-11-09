using Microsoft.EntityFrameworkCore;
using AutoTTU.Connection;
using AutoTTU.Configuration;
using AutoTTU.Services;
using Oracle.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS para permitir acesso do dashboard
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDashboard", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
   options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<MqttOptions>(builder.Configuration.GetSection("Mqtt"));
builder.Services.AddHostedService<MqttTelemetryBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usar CORS
app.UseCors("AllowDashboard");

app.UseAuthorization();

app.MapControllers();

app.Run();
