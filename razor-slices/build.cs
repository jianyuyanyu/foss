// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

#:project ../.github/BuildHelpers/BuildHelpers.csproj

using static BuildHelpers.Targets;

const string TestRazorSlices = "test-razor-slices";
const string TestSourceGenerator = "test-source-generator";

SharedTargets("razor-slices/razor-slices.slnf");

TestTarget(TestRazorSlices, "razor-slices/test/RazorSlices.Tests");
TestTarget(TestSourceGenerator, "razor-slices/test/SourceGenerator.Tests");

DefaultTarget(dependsOn:
[
    CheckFormatting,
    CheckNoChanges,
    TestRazorSlices,
    TestSourceGenerator,
]);

await RunTargetsAndExitAsync(args);
