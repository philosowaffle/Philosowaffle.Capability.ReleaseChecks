namespace Philosowaffle.Capability.ReleaseChecks.Model;

public struct LatestReleaseInformation 
{
	public string? LatestVersion;
	public DateTime ReleaseDate;
	public string? ReleaseUrl;
	public string? Description;
	public bool IsReleaseNewerThanInstalledVersion;
}