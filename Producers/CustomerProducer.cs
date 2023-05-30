
namespace food_delivery.Producers;

using Confluent.Kafka;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using food_delivery.Models;

public class CustomerProducer
{
    private readonly string _bootstrapServers;

    public CustomerProducer(IOptions<KafkaSettings> kafkaSettings)
    {
        _bootstrapServers = kafkaSettings.Value.BootstrapServer;
    }

    public async Task<bool> SendOrderRequest(string topic, string message)
    {
        ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = _bootstrapServers,
            ClientId = Dns.GetHostName()
        };

        try
        {
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var result = await producer.ProduceAsync
                (topic, new Message<Null, string>
                {
                    Value = message
                });

                Debug.WriteLine($"Delivery Timestamp:{ result.Timestamp.UtcDateTime}");
                return await Task.FromResult(true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occured: {ex.Message}");
        }

        return await Task.FromResult(false);
    }

}