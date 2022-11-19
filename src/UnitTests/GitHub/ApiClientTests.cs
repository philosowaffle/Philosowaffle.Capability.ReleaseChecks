﻿using Philosowaffle.Capability.ReleaseChecks.GitHub;
using FluentAssertions;
using Flurl.Http.Testing;
using Moq.AutoMock;
using UnitTests.UnitTestHelpers;

namespace UnitTests.GitHub;
public class ApiClientTests
{
	private string DataDirectory = Path.Join(FileHelper.DataDirectory, "github_responses");

	[Test]
	public async Task GetLatestReleaseAsync_CallsCorrectEndpoint_And_Can_Deserialize()
	{
		var autoMocker = new AutoMocker();
		var apiClient = autoMocker.CreateInstance<ApiClient>();

		var responseData = await FileHelper.ReadTextFromFileAsync(Path.Join(DataDirectory, "latest_release_response.json"));

		var httpMock = new HttpTest();
		httpMock
			.ForCallsTo("https://api.github.com/repos/myOrg/myRepo/releases/latest")
			.WithVerb("GET")
			.RespondWith(responseData, 200, replaceUnderscoreWithHyphen: false);

		var response = await apiClient.GetLatestReleaseAsync("myOrg", "myRepo");

		httpMock.ShouldHaveCalled("https://api.github.com/repos/myOrg/myRepo/releases/latest");
		response.Html_Url.Should().Be("https://github.com/philosowaffle/<ProjectName>/releases/tag/1.0.0");
		response.Published_At.Should().NotBe(DateTime.MinValue);
		response.Tag_Name.Should().Be("1.0.0");
	}
}
