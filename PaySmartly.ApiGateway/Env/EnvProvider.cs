using System.Text.Json;

namespace PaySmartly.ApiGateway.ReverseProxy
{
    public interface IEnvProvider
    {
        IEnumerable<string> GetUIEndpointUrls();
        IEnumerable<string> GetCalculationsEndpointUrls();
        IEnumerable<string> GetArchiveEndpointUrls();
    }

    public class EnvProvider : IEnvProvider
    {
        private const string UI_ENDPOINTS = "UI_ENDPOINTS";

        private const string DEFAULTS_UI_ENDPOINTS = "[\"http://localhost:9081\", \"http://localhost:9082\"]";

        private const string CALCULATIONS_ENDPOINTS = "CALCULATIONS_ENDPOINTS";
        private const string DEFAULTS_CALCULATIONS_ENDPOINTS = "[\"http://localhost:9083/\", \"http://localhost:9084/\"]";

        private const string ARCHIVE_ENDPOINTS = "ARCHIVE_ENDPOINTS";
        private const string DEFAULTS_ARCHIVE_ENDPOINTS = "[\"http://localhost:9085/\", \"http://localhost:9086/\"]";

        public static readonly IEnvProvider Instance;

        static EnvProvider() => Instance = new EnvProvider();

        public IEnumerable<string> GetUIEndpointUrls() => GetEndpointUrls(UI_ENDPOINTS, DEFAULTS_UI_ENDPOINTS);

        public IEnumerable<string> GetCalculationsEndpointUrls() => GetEndpointUrls(CALCULATIONS_ENDPOINTS, DEFAULTS_CALCULATIONS_ENDPOINTS);

        public IEnumerable<string> GetArchiveEndpointUrls() => GetEndpointUrls(ARCHIVE_ENDPOINTS, DEFAULTS_ARCHIVE_ENDPOINTS);

        private IEnumerable<string> GetEndpointUrls(string endpoints, string defaultEndpoints)
        {
            string? json = Environment.GetEnvironmentVariable(endpoints);
            string endpoint = json ?? defaultEndpoints;


            List<string> result = [];
            var urls = JsonSerializer.Deserialize<string[]>(endpoint);

            if (urls is not null)
            {
                foreach (var url in urls)
                {
                    result.Add(url);
                }
            }

            return result;
        }
    }
}