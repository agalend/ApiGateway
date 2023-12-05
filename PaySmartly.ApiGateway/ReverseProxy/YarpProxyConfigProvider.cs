using Yarp.ReverseProxy.Configuration;

namespace PaySmartly.ApiGateway.ReverseProxy
{
    public class YarpProxyConfigProvider : IProxyConfigProvider
    {
        public IProxyConfig GetConfig()
        {
            return new YarpProxyConfig();
        }
    }
}