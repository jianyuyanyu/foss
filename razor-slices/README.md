# Duende.RazorSlices

This is a vendored fork of [DamianEdwards/RazorSlices](https://github.com/DamianEdwards/RazorSlices), maintained here for supply-chain control reasons. Vendoring allows Duende to pin the exact source, apply security patches independently, and ship the package under the `Duende.*` namespace without taking a transitive dependency on an externally-published NuGet package.

The upstream project is copyright Damian Edwards and licensed under the [Apache 2.0 license](../LICENSE). See [ThirdPartyNotices.txt](ThirdPartyNotices.txt) for the full attribution.

## Differences from upstream

- Package ID, assembly name, and root namespace changed from `RazorSlices` to `Duende.RazorSlices`
- Source generator package renamed from `RazorSlices.SourceGenerator` to `Duende.RazorSlices.SourceGenerator`
- Target framework updated to `net10.0`
- Strong-naming disabled (not required for vendored use)
