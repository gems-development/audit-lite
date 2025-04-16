using AuditLiteService;
using AuditLiteService.Data;
using AuditLiteService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var usePostgres = true;

builder.Services.AddGrpc();
builder.Services.AddDbContext<AuditDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
if (usePostgres)
    builder.Services.AddScoped<IAuditEventRepository, PostgresAuditEventRepository>();
else
    builder.Services.AddScoped<IAuditEventRepository, MongoAuditEventRepository>();

var app = builder.Build();
app.Urls.Add("http://localhost:5001");
app.MapGrpcService<AuditLoggerService>();
app.Run();
