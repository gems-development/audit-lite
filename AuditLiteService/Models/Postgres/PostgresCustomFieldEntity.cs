using System.ComponentModel.DataAnnotations;

namespace AuditLiteService.Models;

public class PostgresCustomFieldEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Key { get; set; } = null!;

    [Required]
    public string Value { get; set; } = null!;
    
    public int AuditEventId { get; set; }
    public PostgresAuditEventEntity PostgresAuditEventEntity { get; set; } = null!;

}