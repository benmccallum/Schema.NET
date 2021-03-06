var target = Argument("Target", "Default");
var configuration =
    HasArgument("Configuration") ? Argument<string>("Configuration") :
    EnvironmentVariable("Configuration") != null ? EnvironmentVariable("Configuration") :
    "Release";
var preReleaseSuffix =
    HasArgument("PreReleaseSuffix") ? Argument<string>("PreReleaseSuffix") :
    (AppVeyor.IsRunningOnAppVeyor && AppVeyor.Environment.Repository.Tag.IsTag) ? null :
    EnvironmentVariable("PreReleaseSuffix") != null ? EnvironmentVariable("PreReleaseSuffix") :
    "beta";
var buildNumber =
    HasArgument("BuildNumber") ? Argument<int>("BuildNumber") :
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Number :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Build.BuildNumber :
    EnvironmentVariable("BuildNumber") != null ? int.Parse(EnvironmentVariable("BuildNumber")) :
    0;
var commit =
    HasArgument("Commit") ? Argument<string>("Commit") :
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Repository.Commit.Id :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Repository.Commit :
    EnvironmentVariable("Commit") != null ? EnvironmentVariable("Commit") :
    string.Empty;

var artifactsDirectory = Directory("./Artifacts");
var versionSuffix = string.IsNullOrEmpty(preReleaseSuffix) ? null : preReleaseSuffix + "-" + buildNumber.ToString("D4");

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDirectory);
        DeleteDirectories(GetDirectories("**/bin"), new DeleteDirectorySettings() { Force = true, Recursive = true });
        DeleteDirectories(GetDirectories("**/obj"), new DeleteDirectorySettings() { Force = true, Recursive = true });
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore();
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetCoreBuild(
            ".",
            new DotNetCoreBuildSettings()
            {
                Configuration = configuration,
                NoRestore = true,
                VersionSuffix = versionSuffix
            });
    });

 Task("RunTool")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var project = GetFiles("./**/Schema.NET.Tool.csproj").First();
        DotNetCoreRun(project.ToString());

        Information("Started Listing Files");
        foreach (var file in GetFiles("./**/Schema.NET/**/*"))
        {
            Information(file.ToString());
        }
        Information("Finished Listing Files");
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        foreach(var project in GetFiles("./Tests/**/*.csproj"))
        {
            var outputFilePath = MakeAbsolute(artifactsDirectory.Path)
                .CombineWithFilePath(project.GetFilenameWithoutExtension());
            DotNetCoreTool(
                project,
                "xunit",
                new ProcessArgumentBuilder()
                    .AppendSwitch("-configuration", configuration)
                    .AppendSwitchQuoted("-xml", outputFilePath.AppendExtension(".xml").ToString())
                    .AppendSwitchQuoted("-html", outputFilePath.AppendExtension(".html").ToString()));
        }
    });

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
    {
        DotNetCorePack(
            ".",
            new DotNetCorePackSettings()
            {
                Configuration = configuration,
                NoBuild = true,
                NoRestore = true,
                OutputDirectory = artifactsDirectory,
                VersionSuffix = versionSuffix
            });
    });

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);