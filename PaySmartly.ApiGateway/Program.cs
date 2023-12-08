using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PaySmartly.ApiGateway.ReverseProxy;
using Yarp.ReverseProxy.Configuration;

string ServiceName = "ApiGateway Service";

var builder = WebApplication.CreateSlimBuilder(args);
AddOpenTelemetryLogging(builder);

builder.Services.AddSingleton<IEnvProvider, EnvProvider>();
builder.Services.AddSingleton<IProxyConfigProvider, YarpProxyConfigProvider>();
builder.Services.AddReverseProxy();
AddOpenTelemetryService(builder);

var app = builder.Build();
app.MapReverseProxy();
app.Run();

void AddOpenTelemetryLogging(WebApplicationBuilder builder)
{
   builder.Logging.AddOpenTelemetry(options =>
   {
      ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault().AddService(ServiceName);

      options.SetResourceBuilder(resourceBuilder).AddConsoleExporter();
   });
}

void AddOpenTelemetryService(WebApplicationBuilder builder)
{
   OpenTelemetryBuilder openTelemetryBuilder = builder.Services.AddOpenTelemetry();

   openTelemetryBuilder = openTelemetryBuilder.ConfigureResource(resource => resource.AddService(ServiceName));

   openTelemetryBuilder = openTelemetryBuilder.WithTracing(tracing =>
   {
      tracing.AddAspNetCoreInstrumentation().AddConsoleExporter();
   });
   openTelemetryBuilder = openTelemetryBuilder.WithMetrics(metrics =>
   {
      metrics.AddAspNetCoreInstrumentation().AddConsoleExporter();
   });
}
    