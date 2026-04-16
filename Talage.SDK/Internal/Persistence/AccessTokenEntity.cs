using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Talage.SDK.Internal.Persistence;

[Table("AccessTokens")]
public sealed class AccessTokenEntity
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    public string Token { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("TokenType")]
    public string TokenType { get; set; } = "Bearer";

    [Column("RefreshToken")]
    public string? RefreshToken { get; set; }

    [Column("ExpiresDate")]
    public DateTime ExpiresDate { get; set; }

    [Column("CreatedDate")]
    public DateTime CreatedDate { get; set; }

    [MaxLength(100)]
    public string CreatedBy { get; set; } = "System";

    [Column("LastUpdated")]
    public DateTime? LastUpdated { get; set; }

    [MaxLength(100)]
    [Column("UpdatedBy")]
    public string? UpdatedBy { get; set; }

    public bool IsActive { get; set; }
}

