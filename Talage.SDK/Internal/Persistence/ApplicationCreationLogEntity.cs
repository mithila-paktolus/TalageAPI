using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Talage.SDK.Internal.Persistence;

[Table("ApplicationCreationLogs")]
public sealed class ApplicationCreationLogEntity
{
    [Key]
    [Column("Id")]
    public long Id { get; set; }

    [Column("ApplicationId")]
    public string ApplicationId { get; set; }

    [Column("ResponseJson")]
    public string ResponseJson { get; set; } = string.Empty;

    [Column("IsDeleted")]
    public bool IsDeleted { get; set; }

    [Column("CreatedOn")]
    public DateTime? CreatedOn { get; set; }

    [MaxLength(100)]
    [Column("CreatedBy")]
    public string? CreatedBy { get; set; }

    [Column("LastUpdated")]
    public DateTime? LastUpdated { get; set; }

    [MaxLength(100)]
    [Column("UpdatedBy")]
    public string? UpdatedBy { get; set; }
}

