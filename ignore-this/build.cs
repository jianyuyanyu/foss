// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

#:project ../.github/BuildHelpers/BuildHelpers.csproj

using static BuildHelpers.Targets;

const string Test = "test";

SharedTargets("ignore-this/ignore-this.slnf");

TestTarget(Test, "ignore-this/test/IgnoreThis.Tests");

DefaultTarget(dependsOn:
[
    CheckFormatting,
    CheckNoChanges,
    Test,
]);

await RunTargetsAndExitAsync(args);
