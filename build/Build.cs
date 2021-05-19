using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using System.Linq;

[GitHubActions(
    "deployment",
    GitHubActionsImage.WindowsLatest,
    OnPushBranches = new[] { MasterBranch },
    InvokedTargets = new[] { nameof(Release) },
    ImportGitHubTokenAs = nameof(GitHubToken),
    ImportSecrets =
        new[]
        {
            nameof(NuGetApiKey),
        })]
[GitHubActions(
    "continuous",
    GitHubActionsImage.WindowsLatest,
    OnPushBranchesIgnore = new[] { MasterBranch, ReleaseBranchPrefix + "/*" },
    OnPullRequestBranches = new[] { DevelopBranch, MasterBranch },
    PublishArtifacts = false,
    InvokedTargets = new[] { nameof(Package) })]
[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Package);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] readonly string NuGetApiKey;

    [Parameter] readonly string GitHubToken;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [CI] readonly GitHubActions GitHubActions;

    bool IsOriginalRepository => GitRepository.Identifier == "BerserkerDotNet/BlazorState";
    string NuGetPackageSource => "https://api.nuget.org/v3/index.json";
    string GitHubPackageSource => $"https://nuget.pkg.github.com/{GitHubActions.GitHubRepositoryOwner}/index.json";
    string Source => IsOriginalRepository ? NuGetPackageSource : GitHubPackageSource;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath PackagesDirectory => ArtifactsDirectory / "packages";

    const string MasterBranch = "master";
    const string DevelopBranch = "develop";
    const string ReleaseBranchPrefix = "release";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild());
        });

    Target Package => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetConfiguration(Configuration)
                .SetOutputDirectory(PackagesDirectory)
                .EnableNoBuild()
                .CombineWith(Solution.GetProjects("BlazorState.*"), (s, p) => s.SetProject(p)));
        });

    Target Release => _ => _
        .DependsOn(Package)
        .OnlyWhenDynamic(() => !IsPackageAlreadyPublished())
        .Requires(() => !NuGetApiKey.IsNullOrEmpty())
        .Requires(() => IsOriginalRepository && (GitRepository.IsOnMasterBranch() || GitRepository.IsOnReleaseBranch()))
        .Executes(() =>
        {
            DotNetNuGetPush(s => s
                .SetSource(Source)
                .SetApiKey(NuGetApiKey)
                .CombineWith(PackagesDirectory.GlobFiles("*.nupkg").NotEmpty(), (cs, f) => cs.SetTargetPath(f)),
                degreeOfParallelism: 2);
        });

    private bool IsPackageAlreadyPublished()
    {
        ToolPathResolver.NuGetPackagesConfigFile = Solution.GetProject("_build").Path;
        var output = NuGetTasks.NuGet($"list \"PackageId: BlazorState.Redux\" -PreRelease -Source {Source}", RootDirectory);
        if (output.Count == 0)
        {
            return false;
        }

        var version = output.ElementAt(0).Text.Replace("BlazorState.Redux", string.Empty).Trim();
        var count = PackagesDirectory.GlobFiles($"*{version}*.nupkg").Count;

        return count > 0;
    }
}
