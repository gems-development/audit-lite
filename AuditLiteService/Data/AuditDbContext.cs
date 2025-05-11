using AuditLiteService.Models.Postgres;
using Microsoft.EntityFrameworkCore;

namespace AuditLiteService.Data;

public class AuditDbContext(DbContextOptions<AuditDbContext> options) : DbContext(options)
{
    public DbSet<PostgresAuditEventEntity> AuditEvents { get; set; }
    public DbSet<PostgresEventEnvironmentEntity> EventEnvironments { get; set; }
    public DbSet<PostgresCustomFieldEntity> CustomFields { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostgresAuditEventEntity>()
            .HasOne(a => a.PostgresEventEnvironmentEntity)
            .WithMany()
            .HasForeignKey("EventEnvironmentId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PostgresCustomFieldEntity>()
            .HasOne(cf => cf.PostgresAuditEventEntity)
            .WithMany(a => a.CustomFields)
            .HasForeignKey("AuditEventId")
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}