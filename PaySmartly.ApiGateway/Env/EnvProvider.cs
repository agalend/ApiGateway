using System.Text.Json;
using Microsoft.Extensions.Options;
using PaySmartly.ApiGateway.Env;

namespace PaySmartly.ApiGateway.ReverseProxy
{
    public interface IEnvProvider
    {
        IEnumerable<string> GetUIEndpointUrls();
        IEnumerable<string> GetCalculationsEndpointUrls();
        IEnumerable<string> GetArchiveEndpointUrls();
    }

    public class EnvProvider(IOptions<Endpoints> endpointsSettings) : IEnvProvider
    {
        private const string UI_ENDPOINTS = "UI_ENDPOINTS";
        private const string CALCULATIONS_ENDPOINTS = "CALCULATIONS_ENDPOINTS";
        private const string ARCHIVE_ENDPOINTS = "ARCHIVE_ENDPOINTS";

        private readonly Endpoints endpoints = endpointsSettings.Value;

        public IEnumerable<string> GetUIEndpointUrls() => GetEndpointUrls(UI_ENDPOINTS, endpoints.UI);

        public IEnumerable<string> GetCalculationsEndpointUrls() => GetEndpointUrls(CALCULATIONS_ENDPOINTS, endpoints.Calculations);

        public IEnumerable<string> GetArchiveEndpointUrls() => GetEndpointUrls(ARCHIVE_ENDPOINTS, endpoints.Archive);

        private IEnumerable<string> GetEndpointUrls(string envVar, string[]? defaultUrls)
        {
            string? json = Environment.GetEnvironmentVariable(envVar);

            string[]? urls = json is null
            ? defaultUrls
            : JsonSerializer.Deserialize<string[]>(json);

            return urls ?? Enumerable.Empty<string>();
        }
    }
}