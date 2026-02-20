namespace FLRC.Leaderboards.Importer;

public sealed record CourseImportConfig
{
	public required string Name { get; init; }
	public required string Type { get; init; }
	public required uint ExternalID { get; init; }
	public string? Distance { get; init; }
	public DateTime? Date { get; init; }
}