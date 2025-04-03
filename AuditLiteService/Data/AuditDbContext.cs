using AuditLiteService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuditLiteService.Data;

public class AuditDbContext(DbContextOptions<AuditDbContext> options) : DbContext(options)
{
    public DbSet<AuditEventEntity> AuditEvents { get; set; }
    public DbSet<EventEnvironmentEntity> EventEnvironments { get; set; }
    public DbSet<CustomFieldEntity> CustomFields { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEventEntity>()
            .HasOne(a => a.EventEnvironmentEntity)
            .WithMany()
            .HasForeignKey("EventEnvironmentId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CustomFieldEntity>()
            .HasOne(cf => cf.AuditEventEntity)
            .WithMany(a => a.CustomFields)
            .HasForeignKey("AuditEventId")
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}