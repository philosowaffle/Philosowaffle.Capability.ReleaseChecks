using Flurl.Http;

namespace Philosowaffle.Capability.ReleaseChecks.GitHub;

interface IGitHubApiClient
{
	Task<GitHubLatestReleaseResponse> GetLatestReleaseAsync(string organization, string repository);
}

sealed class ApiClient : IGitHubApiClient
{
	private const string BASE_URL = "https://api.github.com";

	public Task<GitHubLatestReleaseResponse> GetLatestReleaseAsync(string organization, string repository)
	{
		return $"{BASE_URL}/repos/{organization}/{repository}/releases/latest"
			.WithHeader("Accept", "application/json")
			.WithHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:105.0) Gecko/20100101 Firefox/105.0")
			.GetJsonAsync<GitHubLatestReleaseResponse>();
	}
}
