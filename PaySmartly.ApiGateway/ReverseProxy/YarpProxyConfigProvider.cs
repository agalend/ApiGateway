using Yarp.ReverseProxy.Configuration;

namespace PaySmartly.ApiGateway.ReverseProxy
{
     public class YarpProxyConfigProvider(IEnvProvider provider) : IProxyConfigProvider
    {
        private readonly IEnvProvider provider = provider;

        public IProxyConfig GetConfig()
        {
            return new YarpProxyConfig(provider);
        }
    }
}