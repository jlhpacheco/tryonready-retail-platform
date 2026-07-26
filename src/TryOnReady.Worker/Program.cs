using TryOnReady.Worker;
using TryOnReady.Infrastructure;
using TryOnReady.YouCam;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTryOnReadyInfrastructure();
builder.Services.AddYouCamScaffold(builder.Configuration);
builder.Services.AddHostedService<ScaffoldWorker>();

var host = builder.Build();
host.Run();
