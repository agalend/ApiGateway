using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.LoadBalancing;

namespace PaySmartly.ApiGateway.ReverseProxy
{
    public class YarpProxyConfig : IProxyConfig
    {
        private readonly IEnvProvider provider;
        private readonly List<RouteConfig> routes;
        private readonly List<ClusterConfig> clusters;
        private readonly CancellationChangeToken changeToken;
        private readonly CancellationTokenSource cts;

        public YarpProxyConfig(IEnvProvider provider)
        {
            this.provider = provider;
            routes = GenerateRoutes();
            clusters = GenerateClusters();
            cts = new CancellationTokenSource();
            changeToken = new CancellationChangeToken(cts.Token);
        }

        public IReadOnlyList<RouteConfig> Routes => routes;
        public IReadOnlyList<ClusterConfig> Clusters => clusters;
        public IChangeToken ChangeToken => changeToken;

        private List<RouteConfig> GenerateRoutes()
        {   
            List<RouteConfig> collection =
            [
                GenerateRoute("ui", "ui-cluster"),
                GenerateRoute("calculations", "calculations-cluster", "/api/calculations/{**catch-all}"),
                GenerateRoute("archive", "archive-cluster", "/api/archive/{**catch-all}")
            ];

            return collection;
        }

        private List<ClusterConfig> GenerateClusters()
        {
            List<ClusterConfig> collection =
            [
                GenerateCluster("ui-cluster", provider.GetUIEndpointUrls()),
                GenerateCluster("calculations-cluster", provider.GetCalculationsEndpointUrls()),
                GenerateCluster("archive-cluster", provider.GetArchiveEndpointUrls()),
            ];

            return collection;
        }

        private RouteConfig GenerateRoute(string routeId, string clusterId, string path = "{**catch-all}")
        {
            return new()
            {
                RouteId = routeId,
                ClusterId = clusterId,
                Match = new RouteMatch()
                {
                    Path = path
                },
                Transforms = new List<IReadOnlyDictionary<string, string>>()
                {
                    new Dictionary<string, string> { {"PathPattern","{**catch-all}"}}
                }
            };
        }

        private ClusterConfig GenerateCluster(string clusterId, IEnumerable<string> urls)
        {
            Dictionary<string, DestinationConfig> destinations = [];

            int count = 1;
            foreach (var url in urls)
            {
                destinations.Add($"{clusterId}/destination{count}", new DestinationConfig()
                {
                    Address = url,
                    Health = url
                });

                count += 1;
            }

            return new()
            {
                ClusterId = clusterId,
                HealthCheck = new()
                {
                    Active = new()
                    {
                        Enabled = true,
                        Interval = TimeSpan.FromSeconds(1),
                        Timeout = TimeSpan.FromSeconds(3),
                        Policy = "ConsecutiveFailures",
                        Path = "/health"
                    }
                },
                Metadata = new Dictionary<string, string>
                {
                    {"ConsecutiveFailuresHealthPolicy.Threshold", "1"}
                },
                LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                HttpRequest = new ForwarderRequestConfig()
                {
                    VersionPolicy = HttpVersionPolicy.RequestVersionOrLower
                },
                Destinations = destinations
            };
        }
    }
}