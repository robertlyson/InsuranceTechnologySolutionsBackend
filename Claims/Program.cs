using System.Text.Json.Serialization;
using Claims;
using Claims.Application.Covers;
using Domain;
using Infrastructure;
using Infrastructure.Auditing;
using Infrastructure.Covers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);

builder.Services.AddDbContext<AuditContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCosmos();
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(CreateCoverCommand).Assembly));
builder.Services.AddTransient<IPremiumStrategy, DefaultPremiumStrategy>();
builder.Services.AddAuditing();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.RunMigrations();

app.Run();

public partial class Program
{
}