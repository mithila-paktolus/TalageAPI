namespace Talage.SDK.Internal.Auth;

public sealed class TalageApiOptions
{
    public const string SectionName = "TalageApi";

    public string BaseUrl { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;

    public int TokenLifetimeMinutes { get; set; } = 60;
}

