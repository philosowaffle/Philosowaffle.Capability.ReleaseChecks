# Philosowaffle.Capability.ReleaseChecks

Library providing new Release checks capability

## Use

```csharp

// RELEASE CHECKS
builder.Services.AddGitHubReleaseChecker();

var githubService = app.Services.GetService<IGitHubReleaseCheckService>();
var latestVersionInformation = await githubService!.GetLatestReleaseInformationAsync("philosowaffle", "ambientweather-local-server", "0.0.1");

```