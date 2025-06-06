using Microsoft.Extensions.ServiceDiscovery.Http;
using System.Diagnostics;
using System.Net;
using Yarp.ReverseProxy.Forwarder;

namespace Proxy;

public class ForwarderFactory(IServiceDiscoveryHttpMessageHandlerFactory discoveryHttpMessageHandlerFactory) : IForwarderHttpClientFactory
{
    public HttpMessageInvoker CreateClient(ForwarderHttpClientContext context)
    {
        var propagator = DistributedContextPropagator.Current;
        return new HttpMessageInvoker(discoveryHttpMessageHandlerFactory.CreateHandler(new SocketsHttpHandler()
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false,
            EnableMultipleHttp2Connections = true,
            ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
            ConnectTimeout = TimeSpan.FromSeconds(15)
        }));
    }
}
