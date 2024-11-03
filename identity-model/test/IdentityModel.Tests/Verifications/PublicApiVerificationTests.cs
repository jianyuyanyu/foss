using PublicApiGenerator;

namespace Duende.IdentityModel.Verifications;

public class PublicApiVerificationTests
{
    [Fact]
    public async Task VerifyPublicApi()
    {
        var apiGeneratorOptions = new ApiGeneratorOptions
        {
            IncludeAssemblyAttributes = false
        };
        var publicApi = typeof(JwtClaimTypes).Assembly.GeneratePublicApi(apiGeneratorOptions);
        var settings = new VerifySettings();
        settings.UniqueForTargetFrameworkAndVersion();
        await Verify(publicApi, settings);
    }
}