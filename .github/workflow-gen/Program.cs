using Logicality.GitHub.Actions.Workflow;
using System.IO;

void WriteWorkflow(Workflow workflow, string fileName)
{
    var filePath = $"../workflows/{fileName}.yml";
    workflow.WriteYaml(filePath);
    Console.WriteLine($"Wrote workflow to {filePath}");
}


Component[] components = [ 
    new("ignore-this", ["IgnoreThis"], ["IgnoreThis.Tests"]),
];

(string Key, string Value) EnvSecret(string key) => (key, $"${{secrets.{key}}}");


foreach (var component in components)
{
    var workflow = new Workflow($"{component.Name}-ci");
    var paths    = new[] { $".github/workflows/{component.Name}-ci", $"src/{component.Name}/**" };

    workflow.On.WorkflowDispatch();
    workflow.On
        .Push()
        .Branches("main");
    workflow.On
        .PullRequest()
        .Paths(paths);

    workflow.Env(
        ("DOTNETT_NOLOGO", "true"),
        ("DOTNET_CLI_TELEMETRY_OPTOUT", "true"));

    var job = workflow
        .Job("build")
        .Name("Build")
        .RunsOn(GitHubHostedRunners.UbuntuLatest)
        .Defaults().Run("pwsh", component.Name)
        .Job;

    job.Step().ActionsCheckout();

    job.Step().ActionsSetupDotNet("8.0.x");

    foreach(var testProject in component.Tests)
    {
        var path        = $"{component.Name}/test/{testProject}";
        var logFileName = "Tests.trx";
        var flags = $"--logger \"console;verbosity=normal\" "      +
                    $"--logger \"trx;LogFileName={logFileName}\" " +
                    $"--collect:\"XPlat Code Coverage\"";
        job.Step()
            .Name("Test")
            .Run($"dotnet test -c Release {path} {flags}");

        job.Step("test-report")
            .Name("Test report")
            .Uses("dorny/test-reporter@v1")
            .If("success() || failure()")
            .With(
                ("name", "Test Report"),
                ("path", $"{path}/TestResults/{logFileName}"),
                ("reporter", "dotnet-trx"),
                ("fail-on-error", "true"),
                ("fail-on-empty", "true"));
    }

    job.Step()
        .Name("Install Sectigo CodeSiging CA certificates")
        .Run("""
sudo apt-get update
sudo apt-get install -y ca-certificates
sudo cp build/SectigoPublicCodeSigningRootCrossAAA.crt /usr/local/share/ca-certificates/
sudo update-ca-certificates
         
""");
    
    var fileName = $"{component.Name}-ci-gen";

    WriteWorkflow(workflow, fileName);
}

record Component(string Name, string[] Projects, string[] Tests);
