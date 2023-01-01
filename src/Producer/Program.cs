using System.Text;
using NATS.Client;

const string DeviceTopic = "device_topic";

string[] servers = new string[] {
                "nats://localhost:4222",
                "nats://localhost:4223"
            };

Options opts = ConnectionFactory.GetDefaultOptions();
opts.MaxReconnect = 2;
opts.ReconnectWait = 1000;
opts.Servers = servers;

ConnectionFactory cf = new ConnectionFactory();
// connection is expensive to create / should be long lived
var connection = cf.CreateConnection(opts);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/nats", (string message) =>
    {
        SendToNats(DeviceTopic, message);
    })
    .WithName("NatsTest")
    .WithOpenApi();

app.Run();

void SendToNats(string topic, string message)
{
    try
    {
        connection.Publish(topic, Encoding.UTF8.GetBytes(message));
    }
    catch (Exception e)
    {
        Console.WriteLine($"Oops, something went wrong: {e}");
    }
}