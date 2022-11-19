namespace Philosowaffle.Capability.ReleaseChecks.GitHub;

internal sealed record GitHubLatestReleaseResponse
{
	public string? Html_Url { get; init; }
	public string? Tag_Name { get; init; }
	public DateTime Published_At { get; init; }
	public string? Body { get; init; }
}
