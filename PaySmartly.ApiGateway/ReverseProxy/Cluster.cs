namespace PaySmartly.ApiGateway.ReverseProxy
{
    public record Cluster(string Name, string Address, HealthCheck HealthCheck);
}