// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

#:project ../.github/BuildHelpers/BuildHelpers.csproj

using static BuildHelpers.Targets;

const string Test = "test";

SharedTargets("identity-model/identity-model.slnf");

TestTarget(Test, "identity-model/test/IdentityModel.Tests");

DefaultTarget(dependsOn:
[
    CheckFormatting,
    CheckNoChanges,
    Test,
]);

await RunTargetsAndExitAsync(args);
