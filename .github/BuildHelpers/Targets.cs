// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using static Bullseye.Targets;
using static SimpleExec.Command;

namespace BuildHelpers;

/// <summary>
/// Registers build targets for product build scripts.
/// </summary>
public static class Targets
{
    private static readonly Lazy<string> RepoRoot = new(FindRoot);

    private const string Restore = "restore";
    private const string DebugBuild = "debug-build";
    private const string ReleaseBuild = "release-build";

    public const string CheckFormatting = "check-formatting";
    public const string Clean = "clean";
    public const string CheckNoChanges = "check-no-changes";

    private const string Default = "default";

    /// <summary>
    /// Registers all shared targets parameterized by the product's solution filter path.
    /// </summary>
    /// <param name="slnfPath">
    /// Repo-relative path to the product's solution filter
    /// (e.g. <c>access-token-management/access-token-management.slnf</c>).
    /// </param>
    public static void SharedTargets(string slnfPath)
    {
        ArgumentNullException.ThrowIfNull(slnfPath);

        var getChangedCSharpFilesTask = new Lazy<Task<IReadOnlyCollection<string>>>(() =>
            GetChangedCSharpFiles(RepoRoot.Value));

        Target(Restore, () =>
            RunAsync("dotnet", $"restore {slnfPath}", RepoRoot.Value));

        Target(DebugBuild, dependsOn: [Restore], () =>
            RunAsync("dotnet", $"build {slnfPath} --no-restore -c Debug", RepoRoot.Value));

        Target(CheckFormatting, dependsOn: [DebugBuild], async () =>
        {
            var changedCSharpFiles = await getChangedCSharpFilesTask.Value;
            if (changedCSharpFiles.Count == 0)
            {
                await Console.Out.WriteLineAsync("No changed files found.");
                return;
            }

            var include = string.Join(" ", changedCSharpFiles.Select(file => $"\"{file}\""));
            await RunAsync("dotnet", $"format {slnfPath} --verify-no-changes --no-restore --include {include}", RepoRoot.Value);
        });

        Target(Clean, () =>
            RunAsync("dotnet", $"clean {slnfPath}", RepoRoot.Value));

        Target(ReleaseBuild, dependsOn: [Restore], () =>
            RunAsync("dotnet", $"build {slnfPath} --no-restore -c Release", RepoRoot.Value));

        Target(CheckNoChanges, dependsOn: [ReleaseBuild], async () =>
        {
            var (output, _) = await ReadAsync("git", "status --porcelain", workingDirectory: RepoRoot.Value);

            if (!string.IsNullOrWhiteSpace(output))
            {
                await Console.Error.WriteLineAsync("Unexpected changes detected after build:");
                await Console.Error.WriteLineAsync(output);
                throw new InvalidOperationException(
                    "Working tree has uncommitted changes. If these are generated files, commit them before pushing.");
            }
        });
    }

    /// <summary>
    /// Registers a test target that runs <c>dotnet test</c> on a test project with standard options.
    /// </summary>
    /// <param name="targetName">The target name (e.g. <c>"test"</c>).</param>
    /// <param name="testProjectPath">
    /// Repo-relative path to the test project
    /// (e.g. <c>"access-token-management/test/AccessTokenManagement.Tests"</c>).
    /// </param>
    public static void TestTarget(string targetName, string testProjectPath) =>
        Target(targetName, dependsOn: [Restore], () =>
            RunAsync(
                "dotnet",
                $"test --project {testProjectPath} -c Release --no-restore /p:TreatWarningsAsErrors=false --coverage " +
                    $"--report-trx --report-trx-filename {testProjectPath.Replace('/', '-')}-tests.trx",
                RepoRoot.Value));

    public static void DefaultTarget(IEnumerable<string> dependsOn) =>
        Target(Default, dependsOn);

    public static Task RunTargetsAndExitAsync(IEnumerable<string> args) =>
        Bullseye.Targets.RunTargetsAndExitAsync(args, messageOnly: ex => ex is SimpleExec.ExitCodeException);

    private static string FindRoot()
    {
        var root = Directory.GetCurrentDirectory();

        // Repositories have a .git folder, worktrees have a .git file, so check for both.
        while (!Directory.Exists(Path.Combine(root, ".git")) && !File.Exists(Path.Combine(root, ".git")))
        {
            root = Directory.GetParent(root) is { } parent
                ? parent.FullName
                : throw new InvalidOperationException(
                    "Could not find repository root (no .git directory or file found)");
        }

        return root;
    }

    private static async Task<IReadOnlyCollection<string>> GetChangedCSharpFiles(string repoRoot)
    {
        var mainRef = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true"
            ? "origin/main"
            : "main";

        var (mergeBase, _) = await ReadAsync(
            "git", $"merge-base {mainRef} HEAD",
            repoRoot);

        var (committedInBranchOutput, _) = await ReadAsync(
            "git", $"diff --name-only {mergeBase.Trim()}...HEAD",
            repoRoot);

        var (stagedOutput, _) = await ReadAsync(
            "git", "diff --cached --name-only",
            repoRoot);

        var (unstagedOutput, _) = await ReadAsync(
            "git", "diff --name-only",
            repoRoot);

        var committedInBranch = committedInBranchOutput.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
        var staged = stagedOutput.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
        var unstaged = unstagedOutput.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);

        var paths = committedInBranch.Concat(unstaged).Concat(staged)
            .Where(name => string.Equals(Path.GetExtension(name), ".cs", StringComparison.OrdinalIgnoreCase) &&
                           File.Exists(Path.Combine(repoRoot, name)));

        return new HashSet<string>(paths);
    }
}
