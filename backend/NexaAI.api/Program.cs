using NexaAI.Api.Services;
using NexaAI.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Amazon;
using Amazon.SQS;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Resilience;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Content root: {builder.Environment.ContentRootPath}");
var progressConnectionString = builder.Configuration.GetConnectionString("ProgressDatabase")
    ?? throw new InvalidOperationException(
        "The ProgressDatabase connection string is not configured.");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProgressDbContext>(options =>
    options.UseSqlite(progressConnectionString));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IProgressUserContext, LocalProgressUserContext>();
builder.Services.AddScoped<IProgressService, ProgressService>();


builder.Services
    .AddHttpClient<PythonAiService>(client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration["AiService:BaseUrl"]
            ?? "http://127.0.0.1:8000/");

        // The resilience handler owns the request timeouts.
        // Don't set HttpClient.Timeout separately.
    })
    .AddStandardResilienceHandler(options =>
    {
        options.TotalRequestTimeout.Timeout =
            TimeSpan.FromSeconds(20);

        options.AttemptTimeout.Timeout =
            TimeSpan.FromSeconds(8);

        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.MinimumThroughput = 5;
        options.CircuitBreaker.SamplingDuration =
            TimeSpan.FromSeconds(20);
        options.CircuitBreaker.BreakDuration =
            TimeSpan.FromSeconds(15);
    });
// builder.Services.AddhttpClient<AzureAiService>(client =>
// {
//     client.BaseAddress = new Uri("https://nexaaiai.openai.azure.com/");
//     client.Timeout = TimeSpan.FromSeconds(120);
// });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddProblemDetails();



var messagingProvider =
    builder.Configuration["Messaging:Provider"] ?? "AWS";

if (messagingProvider.Equals(
        "AWS",
        StringComparison.OrdinalIgnoreCase))
{
    var awsRegion = builder.Configuration["AWS:Region"]
        ?? throw new InvalidOperationException(
            "AWS:Region is not configured.");

    builder.Services.AddSingleton<IAmazonSQS>(
        _ => new AmazonSQSClient(
            RegionEndpoint.GetBySystemName(awsRegion)));

    builder.Services.AddScoped<
        IAiJobPublisher,
        SqsMessagePublisher>();
}
else if (messagingProvider.Equals(
             "Azure",
             StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton(_ =>
    {
        var connectionString =
            builder.Configuration["AzureServiceBus:ConnectionString"]
            ?? throw new InvalidOperationException(
                "AzureServiceBus:ConnectionString is not configured.");

        return new ServiceBusClient(connectionString);
    });

    builder.Services.AddScoped<
        IAiJobPublisher,
        AzureServiceBusPublisher>();
}
else
{
    throw new InvalidOperationException(
        $"Unsupported messaging provider: {messagingProvider}");
}
var app = builder.Build();
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature =
            context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

        var logger = context.RequestServices
            .GetRequiredService<ILogger<Program>>();

        if (exceptionFeature?.Error is Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled exception for {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
        }

        context.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(new
        {
            title = "An unexpected server error occurred.",
            status = StatusCodes.Status500InternalServerError,
            traceId = context.TraceIdentifier
        });
    });
});
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AngularDev");
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => new
{
    application = "NexaAI API",
    status = "Running"
});

app.Run();