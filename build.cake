#addin nuget:?package=SharpZipLib
#addin nuget:?package=Cake.Compression

#tool "nuget:?package=NuGet.CommandLine&version=6.12.2"

using System.Text.RegularExpressions;

var target = Argument("target", "Default");

// arguments
var url = Argument<string>("url");
var fileName = new Uri(url).Segments.Last();
var apiKey = Argument<string>("api-key", "");
var version = Argument("packageVersion", GetVersionFromFile(url));

// variables
const string downloadDir = "./download/";
const string extractDir = "./extracted/";
const string nugetOutDir = "./nuget";
const string nugetRepository = "https://www.nuget.org/api/v2/package";
// helpers
private string GetVersionFromFile(string fileName)
{
    var versionMatch = Regex.Match(fileName, @"liquibase-(([0-9]+\.)+[0-9]+)");
    if (versionMatch.Success)
    {
        return versionMatch.Groups[1].Value;
    } else
    {
        return "0.0.0.0";
    }
}

// tasks
Task("Clean")
    .Description("Cleans the working dir to start in a fresh environment.")
    .Does(() => {
        CleanDirectory(extractDir);
        CleanDirectory(nugetOutDir);
    });

Task("Prepare")
    .Description("Downloads the liquibase distribution and unpacks it in the working dir.")
    .IsDependentOn("Clean")
    .Does(() => {
        if (!fileName.EndsWith(".tar.gz"))
            throw new Exception("URI does not seem to be a downloadable tar.gz file.");
        
        var outputPath = File(downloadDir + fileName);

        if (!FileExists(outputPath)){
            Information("Liquibase tar file not in download directory. Downloading now ...");
            EnsureDirectoryExists(downloadDir);
            DownloadFile(url, outputPath);
        }

        Information("Uncompressing tar file ...");
        GZipUncompress(outputPath, extractDir);
    });

Task("Pack")
    .Description("Packs the liquibase distribution in a nuget package.")
    .IsDependentOn("Prepare")
    .Does(() => {

        var dotnetBuildSettings = new DotNetBuildSettings
        {
            Configuration = "Release",
            //NoPackageAnalysis = true,
            OutputDirectory = nugetOutDir,
            ArgumentCustomization = a => a.Append($"-p:Version={version}").Append($"-p:NuspecBasePath={extractDir}")
        };

        DotNetBuild("Liquibase.csproj", dotnetBuildSettings);
    });

Task("Push")
    .Description("Pushes the NuGet package to nuget.org.")
    .IsDependentOn("Pack")
    .Does(() => {
        if (string.IsNullOrEmpty(apiKey)) 
        {
            throw new Exception("The api-key parameter is required to push a package.");
        }

        var packages = GetFiles("./nuget/*.nupkg");

        NuGetPush(packages, new NuGetPushSettings {
            Source = nugetRepository,
            ApiKey = apiKey
        });
    });

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);
