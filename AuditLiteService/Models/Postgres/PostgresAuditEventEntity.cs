using System.ComponentModel.DataAnnotations;

namespace AuditLiteService.Models.Postgres;

public class PostgresAuditEventEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string EventType { get; set; } = null!;

    [Required]
    public DateTime EventDate { get; set; }
    
    public int EventEnvironmentId { get; set; }
    
    public PostgresEventEnvironmentEntity PostgresEventEnvironmentEntity { get; set; } = null!;
    
    public List<PostgresCustomFieldEntity> CustomFields { get; set; } = new();
}