using Core.GitHub;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.AutoMock;
using Philosowaffle.Capability.ReleaseChecks.GitHub;

namespace UnitTests.GitHub;

public class GitHubServiceTests
{
	[Test]
	public async Task GetLatestReleaseAsync_MapsData_Correctly()
	{
		// SETUP
		var autoMocker = new AutoMocker();
		var cache = new MemoryCache(new MemoryCacheOptions());
		autoMocker.Use<IMemoryCache>(cache);

		var service = autoMocker.CreateInstance<GitHubReleaseCheckService>();
		var api = autoMocker.GetMock<IGitHubApiClient>();

		var gitHubResponse = new GitHubLatestReleaseResponse()
		{
			Tag_Name = "0.3.4",
			Body = "Release notes",
			Html_Url = "https://www.google.com",
			Published_At = DateTime.Now
		};

		api.Setup(x => x.GetLatestReleaseAsync("myOrg", "myRepo"))
			.ReturnsAsync(gitHubResponse)
			.Verifiable();

		// ACT
		var result = await service.GetLatestReleaseInformationAsync("myOrg", "myRepo", "0.4.0");

		// ASSERT
		result.LatestVersion.Should().Be(gitHubResponse.Tag_Name);
		result.ReleaseDate.Should().Be(gitHubResponse.Published_At);
		result.ReleaseUrl.Should().Be(gitHubResponse.Html_Url);
		result.Description.Should().Be(gitHubResponse.Body);
		result.IsReleaseNewerThanInstalledVersion.Should().BeFalse();

		api.Verify();
	}

	[Test]
	public async Task GetLatestReleaseAsync_ReturnsDefault_When_ExceptionThrown()
	{
		// SETUP
		var autoMocker = new AutoMocker();
		var cache = new MemoryCache(new MemoryCacheOptions());
		autoMocker.Use<IMemoryCache>(cache);

		var service = autoMocker.CreateInstance<GitHubReleaseCheckService>();
		var api = autoMocker.GetMock<IGitHubApiClient>();

		api.Setup(x => x.GetLatestReleaseAsync("myOrg", "myRepo"))
			.ThrowsAsync(new Exception())
			.Verifiable();

		// ACT
		var result = await service.GetLatestReleaseInformationAsync("myOrg", "myRepo", "0.2.0");

		// ASSERT
		result.LatestVersion.Should().BeNull();
		result.ReleaseDate.Should().Be(DateTime.MinValue);
		result.ReleaseUrl.Should().BeNull();
		result.Description.Should().BeNull();
		result.IsReleaseNewerThanInstalledVersion.Should().BeFalse();

		api.Verify();
	}

	[TestCase(null, null, ExpectedResult = false)]
	[TestCase(null, "1.0.0", ExpectedResult = false)]
	[TestCase("1.0.0", null, ExpectedResult = false)]

	[TestCase("", "", ExpectedResult = false)]
	[TestCase("", "1.0.0", ExpectedResult = false)]
	[TestCase("1.0.0", "", ExpectedResult = false)]

	[TestCase(" ", " ", ExpectedResult = false)]
	[TestCase(" ", "1.0.0", ExpectedResult = false)]
	[TestCase("1.0.0", " ", ExpectedResult = false)]

	[TestCase("abacva", "abavad", ExpectedResult = false)]
	[TestCase("abacva", "1.0.0", ExpectedResult = false)]
	[TestCase("1.0.0", "abacva", ExpectedResult = false)]

	[TestCase("0.0.1", "1.0.0", ExpectedResult = false)]
	[TestCase("v0.0.1", "1.0.0", ExpectedResult = false)]
	[TestCase("0.1.1", "1.0.0", ExpectedResult = false)]
	[TestCase("1.0.0", "1.0.0", ExpectedResult = false)]

	[TestCase("1.0.1", "1.0.0", ExpectedResult = true)]
	[TestCase("1.1.0", "1.0.0", ExpectedResult = true)]
	[TestCase("8.0.1", "1.0.0", ExpectedResult = true)]

	[TestCase("1.0.0", "1.0.0-rc", ExpectedResult = true)]
	[TestCase("0.9.0", "1.0.0-rc", ExpectedResult = false)]
	public async Task<bool> GetLatestReleaseAsync_Calculates_IsNewVersion_Correctly(string? ghVersion, string? installedVersion)
	{
		// SETUP
		var autoMocker = new AutoMocker();
		var cache = new MemoryCache(new MemoryCacheOptions());
		autoMocker.Use<IMemoryCache>(cache);

		var service = autoMocker.CreateInstance<GitHubReleaseCheckService>();
		var api = autoMocker.GetMock<IGitHubApiClient>();

		var gitHubResponse = new GitHubLatestReleaseResponse()
		{
			Tag_Name = ghVersion,
		};

		api.Setup(x => x.GetLatestReleaseAsync("myOrg", "myRepo"))
			.ReturnsAsync(gitHubResponse)
			.Verifiable();

		// ACT
		var result = await service.GetLatestReleaseInformationAsync("myOrg", "myRepo", installedVersion!);

		// ASSERT
		if (installedVersion is not null
			&& !string.IsNullOrWhiteSpace(installedVersion))
			api.Verify();
		
		return result.IsReleaseNewerThanInstalledVersion;
	}
}
