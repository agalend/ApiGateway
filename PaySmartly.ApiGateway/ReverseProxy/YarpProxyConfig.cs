using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace PaySmartly.ApiGateway.ReverseProxy
{
    public class YarpProxyConfig : IProxyConfig
    {
        private readonly List<RouteConfig> routes;
        private readonly List<ClusterConfig> clusters;
        private readonly CancellationChangeToken changeToken;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public YarpProxyConfig()
        {
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
            var collection = new List<RouteConfig>
            {
                new()
                {
                    RouteId = "app",
                    ClusterId = "app-cluster",
                    Match = new RouteMatch()
                    {
                        Path = "{**catch-all}"
                    },
                    Transforms = new List<IReadOnlyDictionary<string, string>>()
                    {
                        new Dictionary<string, string> { {"PathPattern","{**catch-all}"}}
                    }
                },
                new()
                {
                    RouteId = "calculations",
                    ClusterId = "calculations-cluster",
                    Match = new RouteMatch()
                    {
                        Path = "/api/calculations/{**catch-all}"
                    },
                    Transforms = new List<IReadOnlyDictionary<string, string>>()
                    {
                        new Dictionary<string, string> { {"PathPattern","{**catch-all}"}}
                    }
                },
                new()
                {
                    RouteId = "archive",
                    ClusterId = "archive-cluster",
                    Match = new RouteMatch()
                    {
                        Path = "/api/archive/{**catch-all}"
                    },
                    Transforms = new List<IReadOnlyDictionary<string, string>>()
                    {
                        new Dictionary<string, string> { {"PathPattern","{**catch-all}"}}
                    }
                }
            };

            return collection;
        }

        private List<ClusterConfig> GenerateClusters()
        {
            var collection = new List<ClusterConfig>
            {
                new()
                {
                    ClusterId = "app-cluster",
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        {
                            "server", new DestinationConfig()
                            {
                                Address = "http://localhost:9081"
                            }
                        }
                    }
                },
                new()
                {
                    ClusterId = "calculations-cluster",
                    HealthCheck = new()
                    {
                        Active = new()
                        {
                            Enabled = true,
                            Interval = TimeSpan.FromSeconds(5),
                            Timeout = TimeSpan.FromSeconds(5),
                            Policy = "ConsecutiveFailures",
                            Path = "/health"
                        }
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        {"ConsecutiveFailuresHealthPolicy.Threshold", "5"}
                    },
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        {
                            "server", new DestinationConfig()
                            {
                                Address = "http://localhost:9083/",
                                Health = "http://localhost:9083/"
                            }
                        }
                    }
                },
                new()
                {
                    ClusterId = "archive-cluster",
                    HealthCheck = new()
                    {
                        Active = new()
                        {
                            Enabled = true,
                            Interval = TimeSpan.FromSeconds(5),
                            Timeout = TimeSpan.FromSeconds(5),
                            Policy = "ConsecutiveFailures",
                            Path = "/health"
                        }
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        {"ConsecutiveFailuresHealthPolicy.Threshold", "5"}
                    },
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        {
                            "server", new DestinationConfig()
                            {
                                Address = "http://localhost:9085/",
                                Health = "http://localhost:9085/"
                            }
                        }
                    }
                }
            };

            return collection;
        }
    }
}