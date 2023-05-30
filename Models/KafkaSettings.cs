namespace food_delivery.Models;

public class KafkaSettings
{
    public string BootstrapServer { get; set; } = null!;
    public string MenuUpdatesTopic { get; set; } = null!;
    public string OrdersTopic { get; set; } = null!;
    
}