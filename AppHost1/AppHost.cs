var builder = DistributedApplication.CreateBuilder(args);

var proxy = builder.AddProject<Projects.Proxy>("proxy");
#if DEBUG
var fe = builder.AddProject<Projects.BlazorApp1>("blazorapp1");
proxy.WithReference(fe);
#endif

builder.Build().Run();
