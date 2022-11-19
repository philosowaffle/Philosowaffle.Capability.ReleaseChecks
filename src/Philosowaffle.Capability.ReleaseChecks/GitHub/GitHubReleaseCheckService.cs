using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Philosowaffle.Capability.ReleaseChecks.GitHub;
using Philosowaffle.Capability.ReleaseChecks.Model;

namespace Core.GitHub;

public interface IGitHubReleaseCheckService
{
	Task<LatestReleaseInformation> GetLatestReleaseInformationAsync(string organization, string repository, string installedVersion);
}

internal sealed class GitHubReleaseCheckService : IGitHubReleaseCheckService
{ 
	private static readonly object _lock = new object();

	private const string LatestReleaseKey = "GithubLatestRelease";

	private readonly ILogger _logger;
	private readonly IGitHubApiClient _apiClient;
	private readonly IMemoryCache _cache;

	public GitHubReleaseCheckService(ILogger<GitHubReleaseCheckService> logger, IGitHubApiClient apiClient, IMemoryCache cache)
	{
		_logger = logger;
		_apiClient = apiClient;
		_cache = cache;
	}

	public Task<LatestReleaseInformation> GetLatestReleaseInformationAsync(string organization, string repository, string installedVersion)
	{
		// TODO: guard library
		if (string.IsNullOrWhiteSpace(installedVersion))
		{
			_logger.LogTrace("installedVersion is null");
			return Task.FromResult(new LatestReleaseInformation());
		}

		// TODO: when pulling in tracing library
		//using var tracing = Tracing.Trace($"{nameof(GitHubService)}.{nameof(GetLatestReleaseAsync)}");

		lock (_lock)
		{
			return _cache.GetOrCreateAsync(LatestReleaseKey, async (cacheEntry) =>
			{
				cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

				try
				{
					var latestVersionInformation = await _apiClient.GetLatestReleaseAsync(organization, repository);
					var newVersionAvailable = IsReleaseNewerThanInstalledVersion(latestVersionInformation.Tag_Name, installedVersion, _logger);

					// TODO: when pulling in metrics library
					//AppMetrics.SyncUpdateAvailableMetric(newVersionAvailable, latestVersionInformation.Tag_Name);

					return new LatestReleaseInformation()
					{
						LatestVersion = latestVersionInformation.Tag_Name,
						ReleaseDate = latestVersionInformation.Published_At,
						ReleaseUrl = latestVersionInformation.Html_Url,
						Description = latestVersionInformation.Body,
						IsReleaseNewerThanInstalledVersion = newVersionAvailable
					};
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Error occurred while checking for new release.");
					cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
					return new LatestReleaseInformation();
				}
			});
		}
	}

	private static bool IsReleaseNewerThanInstalledVersion(string? releaseVersion, string? currentVersion, ILogger logger)
	{
		// TOOD: guard api
		if (string.IsNullOrEmpty(releaseVersion))
		{
			logger.LogTrace("Latest Release version from GitHub was null");
			return false;
		}

		if (string.IsNullOrEmpty(currentVersion))
		{
			logger.LogTrace("Current install version is null");
			return false;
		}

		var isCurrentVersionRC = currentVersion.Trim().ToLower().Contains("-rc");

		var cleanedReleaseVersion = releaseVersion.Trim().ToLower().Replace("v", string.Empty);
		var cleanedInstalledVersion = currentVersion.Trim().ToLower().Replace("-rc", string.Empty);

		if (!Version.TryParse(cleanedReleaseVersion, out var latestVersion))
		{
			logger.LogTrace("Failed to parse latest release version: {@Version}", cleanedReleaseVersion);
			return false;
		}

		if (!Version.TryParse(cleanedInstalledVersion, out var installedVersion))
		{
			logger.LogTrace("Failed to parse installed version: {@Version}", cleanedInstalledVersion);
			return false;
		}

		if (isCurrentVersionRC)
			return latestVersion >= installedVersion;

		return latestVersion > installedVersion;
	}
}
