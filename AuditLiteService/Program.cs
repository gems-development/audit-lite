using AuditLiteService.Data;
using AuditLiteService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddDbContext<AuditDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

var app = builder.Build();
app.Urls.Add("http://localhost:5001");
app.MapGrpcService<AuditLoggerService>();
app.Run();
