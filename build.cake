#load "./cake/SharpBuild.cake"

var build = new SharpBuild(Context, "Zelyutin", "SampSharp.RakNet",
    "SampSharp.RakNet");

Task("Clean")
    .Does(() => build.Clean());
   
Task("Restore")
    .Does(() => build.Restore());
 
Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => build.Build());
    
Task("Pack")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => build.Pack());

Task("Publish")
    .WithCriteria(() => build.IsAppVeyorTag)
    .IsDependentOn("Pack")
    .Does(() => build.Publish());

Task("Default")
    .IsDependentOn("Build");

Task("AppVeyor")
    .IsDependentOn("Publish");

RunTarget(Argument("target", "Default"));