# Philosowaffle.Capability.ReleaseChecks
[![](https://img.shields.io/static/v1?label=Sponsor&message=%E2%9D%A4&logo=GitHub&color=%23fe8e86)](https://github.com/sponsors/philosowaffle)
<span class="badge-buymeacoffee"><a href="https://www.buymeacoffee.com/philosowaffle" title="Donate to this project using Buy Me A Coffee"><img src="https://img.shields.io/badge/buy%20me%20a%20coffee-donate-yellow.svg" alt="Buy Me A Coffee donate button" /></a></span> [![GitHub license](https://img.shields.io/github/license/philosowaffle/Philosowaffle.Capability.ReleaseChecks.svg)](https://github.com/philosowaffle/Philosowaffle.Capability.ReleaseChecks/blob/master/LICENSE)
[![GitHub Release](https://img.shields.io/github/release/philosowaffle/Philosowaffle.Capability.ReleaseChecks.svg?style=flat)](https://github.com/philosowaffle/Philosowaffle.Capability.ReleaseChecks/releases)

Library providing new Release checks capability

## Use

```csharp

// RELEASE CHECKS
builder.Services.AddGitHubReleaseChecker();

var githubService = app.Services.GetService<IGitHubReleaseCheckService>();
var latestVersionInformation = await githubService!.GetLatestReleaseInformationAsync("philosowaffle", "ambientweather-local-server", "0.0.1");

```
