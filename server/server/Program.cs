using Grpc.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using server.Services;

var builder = WebApplication.CreateBuilder();
builder.Services.AddGrpc();
builder.WebHost.ConfigureKestrel(k =>
{
    k.ConfigureEndpointDefaults(options => options.Protocols = HttpProtocols.Http2);
    k.ListenLocalhost(5500, o => o.UseHttps());
});

var app = builder.Build();

app.MapGrpcService<CategoriesService>();
app.MapGrpcService<RecipesService>();
app.MapGet("/", () => "This server contains a gRPC service");

app.Run();
