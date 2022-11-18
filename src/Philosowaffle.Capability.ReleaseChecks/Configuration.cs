using Core.GitHub;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Philosowaffle.Capability.ReleaseChecks.GitHub;

namespace Philosowaffle.Capability.ReleaseChecks;

public static class Configuration
{
	public static IServiceCollection AddGitHubReleaseChecker(this IServiceCollection services)
	{
		services.AddSingleton<IMemoryCache, MemoryCache>();
		services.AddSingleton<IGitHubApiClient, ApiClient>();
		services.AddSingleton<IGitHubReleaseCheckService, GitHubReleaseCheckService>();

		return services;
	}
}