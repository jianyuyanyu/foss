#if NETFRAMEWORK
using System.Net.Http;
#endif

namespace Duende.IdentityModel.HttpClientExtensions;

internal static class HttpRequestMethodExtensions
{
    public static IDictionary<string, object> GetProperties(this HttpRequestMessage requestMessage)
    {
#if NETFRAMEWORK
        return requestMessage.Properties;
#else
        return (IDictionary<string, object>)requestMessage.Options;
#endif
    }
}