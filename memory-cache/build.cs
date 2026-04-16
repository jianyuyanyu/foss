// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

#:project ../.github/BuildHelpers/BuildHelpers.csproj

using static BuildHelpers.Targets;

const string Test = "test";

SharedTargets("memory-cache/memory-cache.slnf");

TestTarget(Test, "memory-cache/test/Extensions.Caching.Memory.Tests");

DefaultTarget(dependsOn:
[
    CheckFormatting,
    CheckNoChanges,
    Test,
]);

await RunTargetsAndExitAsync(args);
