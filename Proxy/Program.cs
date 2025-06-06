#if RELEASE
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
#endif
using Microsoft.Extensions.ServiceDiscovery;
using Proxy;
using Yarp.ReverseProxy.Forwarder;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddServiceDiscovery();
builder.Services.ConfigureHttpClientDefaults(static http =>
{
    // Turn on service discovery by default
    http.AddServiceDiscovery();
});
builder.Services.Configure<ServiceDiscoveryOptions>(
    options => options.AllowAllSchemes = true);
builder.WebHost.UseKestrelHttpsConfiguration();
builder.Services.AddSingleton<IForwarderHttpClientFactory, ForwarderFactory>();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy")); ;
var app = builder.Build();
#if RELEASE
app.UseDefaultFiles();
app.UseBlazorFrameworkFiles("/blazorapp1");
app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = (c) =>
    {
        c.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(1),
            MustRevalidate = true
        };
        //if (!c.File.Exists)
        //{
        //    c.File = new FileInfo("index.html");
        //}
    },
    HttpsCompression = HttpsCompressionMode.Compress,
    ServeUnknownFileTypes = true
});
app.MapFallbackToFile("/blazorapp1/{**t}", "/blazorapp1/index.html");
#endif
app.UseRouting();
app.MapReverseProxy();
await app.RunAsync();