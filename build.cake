#addin nuget:?package=SharpZipLib
#addin nuget:?package=Cake.Compression

var target = Argument("target", "Default");

// arguments
var url = Argument<string>("url");
var apiKey = Argument<string>("api-key");

// variables
const string downloadDir = "./download/";
const string extractDir = "./extracted/";

var fileName = new Uri(url).Segments.Last();

Task("Clean")
    .Description("Cleans the working dir to start in a fresh environment.")
    .Does(() => {
        CleanDirectory(extractDir);
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
        
    });

Task("Push")
    .Description("Pushes the NuGet package to nuget.org.")
    .Does(() => {

    });

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);
