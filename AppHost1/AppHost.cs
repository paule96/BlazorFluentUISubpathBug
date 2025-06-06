var builder = DistributedApplication.CreateBuilder(args);

var profile = "Development";
#if RELEASE
profile = "Release";
#endif

var proxy = builder.AddProject<Projects.Proxy>("proxy", profile);
#if DEBUG
var fe = builder.AddProject<Projects.BlazorApp1>("blazorapp1");
proxy.WithReference(fe);
#endif

builder.Build().Run();
