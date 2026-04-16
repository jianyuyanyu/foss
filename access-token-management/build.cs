// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

#:project ../.github/BuildHelpers/BuildHelpers.csproj

using static BuildHelpers.Targets;

const string Test = "test";

SharedTargets("access-token-management/access-token-management.slnf");

TestTarget(Test, "access-token-management/test/AccessTokenManagement.Tests");

DefaultTarget(dependsOn:
[
    CheckFormatting,
    CheckNoChanges,
    Test,
]);

await RunTargetsAndExitAsync(args);
