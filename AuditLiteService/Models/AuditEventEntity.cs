using System.ComponentModel.DataAnnotations;

namespace AuditLiteService.Models;

public class AuditEventEntity
{
    [Key]
    public int Id { get; set; }  // Первичный ключ

    [Required]
    public string EventType { get; set; } = null!;

    [Required]
    public DateTime EventDate { get; set; }
    
    public int EventEnvironmentId { get; set; }
    public EventEnvironmentEntity EventEnvironmentEntity { get; set; } = null!;
    
    public List<CustomFieldEntity> CustomFields { get; set; } = new();
}