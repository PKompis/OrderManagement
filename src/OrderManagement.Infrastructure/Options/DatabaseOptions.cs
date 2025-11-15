namespace OrderManagement.Infrastructure.Options;

/// <summary>
/// Database Options
/// </summary>
public sealed class DatabaseOptions
{
    /// <summary>
    /// The section name
    /// </summary>
    public const string SectionName = "Database";

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    public string? ConnectionString { get; init; }
}
