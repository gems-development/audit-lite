using AuditLiteService;
using AuditLiteService.Data;
using AuditLiteService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddDbContext<AuditDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
builder.Services.AddScoped<MongoAuditEventRepository>();
builder.Services.AddScoped<PostgresAuditEventRepository>();
builder.Services.AddScoped<IAuditRepositoryFactory, AuditRepositoryFactory>();

var app = builder.Build();
app.Urls.Add("http://localhost:5001");
app.MapGrpcService<AuditLoggerService>();
app.Run();
