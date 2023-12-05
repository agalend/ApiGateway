namespace PaySmartly.ApiGateway.ReverseProxy
{
    public record HealthCheck(string Interval, string Timeout, string Address, string Path);
}