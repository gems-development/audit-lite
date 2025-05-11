using System.ComponentModel.DataAnnotations;

namespace AuditLiteService.Models.Postgres;

public class PostgresEventEnvironmentEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    public string MethodName { get; set; } = null!;

    [Required]
    public string MachineName { get; set; } = null!;

    [Required]
    public string IpAddress { get; set; } = null!;
}