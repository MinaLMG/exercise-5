using Grpc.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using server.Services;

var builder = WebApplication.CreateBuilder();

builder.Services.AddGrpc();
//builder.WebHost.ConfigureKestrel(k =>
//{
//    k.ConfigureEndpointDefaults(options => options.Protocols = HttpProtocols.Http2);
//    k.ListenLocalhost(5500, o => o.UseHttps());
//});
builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));


var app = builder.Build();


app.UseRouting();
app.UseGrpcWeb();
app.UseCors();

app.MapGrpcService<CategoriesService>().EnableGrpcWeb().RequireCors("AllowAll");
app.MapGrpcService<RecipesService>().EnableGrpcWeb().RequireCors("AllowAll");

app.MapGet("/", () => "This server contains a gRPC service");
app.Run();
