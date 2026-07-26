using TryOnReady.Worker;
using TryOnReady.Infrastructure;
using TryOnReady.YouCam;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTryOnReadyInfrastructure(builder.Configuration);
builder.Services.AddYouCam(builder.Configuration);
builder.Services.AddHostedService<ScaffoldWorker>();

var host = builder.Build();
host.Run();
