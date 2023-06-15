using food_delivery.Services;
using food_delivery.Models;
using food_delivery.Producers;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<FoodDeliveryDatabaseSettings>(builder.Configuration.GetSection("FoodDeliveryDatabase"));
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddSingleton<IMerchantsService, MerchantsService>();
builder.Services.AddSingleton<CustomerProducer>();
builder.Services.AddSingleton<StripeProductsService>();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe").GetValue<string>("ApiKey");

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    builder.Services.AddCors();
    app.UseCors(options => options
        // .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins(
            "http://food-delivery.lcl:30011", // kubernetes local
            "http://localhost:3001" // development
            )
        );

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseHttpsRedirection();

app.UseAuthorization();


app.Run();
