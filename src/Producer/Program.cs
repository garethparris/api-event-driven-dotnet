using Confluent.Kafka;

const string DeviceTopic = "device_topic";
ProducerConfig config = new ProducerConfig { BootstrapServers = "localhost:9092" };

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

app.MapPost("/api/kafka", async (string message) =>
    {
        await SendToKafka(DeviceTopic, message);
    })
    .WithName("KafkaTest")
    .WithOpenApi();

app.Run();

async Task SendToKafka(string topic, string message)
{
    using var producer = new ProducerBuilder<Null, string>(config).Build();
        
    try
    {
        await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
    }
    catch (Exception e)
    {
        Console.WriteLine($"Oops, something went wrong: {e}");
    }
}