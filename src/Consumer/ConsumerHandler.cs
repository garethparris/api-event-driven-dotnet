using System.Text;
using NATS.Client;

namespace Consumer;

public class ConsumerHandler : IHostedService
{
    private const string Topic = "device_topic";

    public Task StartAsync(CancellationToken cancellationToken)
    {
        string[] servers = new string[]
        {
            "nats://localhost:4222",
            "nats://localhost:4223"
        };

        Options opts = ConnectionFactory.GetDefaultOptions();
        opts.MaxReconnect = 2;
        opts.ReconnectWait = 1000;
        opts.Servers = servers;

        ConnectionFactory cf = new ConnectionFactory();

        try
        {
            var connection = cf.CreateConnection(opts);
            var asyncSubscription = connection.SubscribeAsync(Topic);
            
            asyncSubscription.MessageHandler += (_, e) =>
            {
                Console.WriteLine($"Message received: {Encoding.UTF8.GetString(e.Message.Data)}");
            };
            
            asyncSubscription.Start();

            var consoleKeyInfo = new ConsoleKeyInfo();

            do
            {
                if (Console.KeyAvailable)
                {
                    consoleKeyInfo = Console.ReadKey();
                }
            } while (consoleKeyInfo.Key != ConsoleKey.Escape);
            
            asyncSubscription.Unsubscribe();
            
            connection.Drain();
            connection.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Oops, something went wrong: {e}");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}