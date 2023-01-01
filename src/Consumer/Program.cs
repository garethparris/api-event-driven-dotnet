using Consumer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IHostedService, ConsumerHandler>();

var app = builder.Build();
app.Run();
