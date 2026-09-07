using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Vitalia.Infrastructure.Data;
using Vitalia.API.Exceptions;
using Vitalia.API.Configurations;
using Vitalia.API.Health;
using Vitalia.API.Extensions;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "Vitalia.API")
        .WriteTo.Console();
});

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService("Vitalia.API"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter(
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Routing",
                "Microsoft.AspNetCore.Server.Kestrel",
                "System.Net.Http",
                "System.Runtime")
            .AddPrometheusExporter();
    });

const string CorsPolicyName = "VitaliaFrontend";

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

JwtConfiguration.Configure(
    builder.Services,
    builder.Configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vitalia",
        Version = "v1",
        Description =
            "API do módulo comercial da plataforma Vitalia para gerenciamento de categorias, produtos, carrinho, checkout e pedidos."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
            "Informe o token JWT no formato: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var connectionString = builder.Configuration.GetConnectionString("OracleConnection")
                       ?? throw new InvalidOperationException(
                           "A connection string 'OracleConnection' não foi encontrada."
                       );

builder.Services.AddDbContext<VitaliaDbContext>(options =>
    options.UseOracle(connectionString));

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<VitaliaDbContext>(
        name: "Oracle",
        tags: new[] { "ready" });

builder.Services.AddVitaliaServices();

var app = builder.Build();

app.UseExceptionHandler();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} " +
        "responded {StatusCode} in {Elapsed:0.0000} ms | " +
        "TraceId: {TraceId} | " +
        "RequestId: {RequestId}";

    options.EnrichDiagnosticContext =
        (diagnosticContext, httpContext) =>
        {
            var traceId =
                System.Diagnostics.Activity.Current?
                    .TraceId
                    .ToString()
                ?? httpContext.TraceIdentifier;

            diagnosticContext.Set(
                "TraceId",
                traceId);

            diagnosticContext.Set(
                "RequestId",
                httpContext.TraceIdentifier);
        };
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Vitalia v1"
        );

        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    application = "Vitalia API",
    status = "running",
    environment = app.Environment.EnvironmentName,
    links = new
    {
        swagger = "/swagger",
        health = "/health",
    }
}));

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = healthCheck =>
        healthCheck.Tags.Contains("ready"),

    ResponseWriter =
        HealthCheckResponseWriter.WriteJsonResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,

    ResponseWriter =
        HealthCheckResponseWriter.WriteJsonResponse
});

app.Run();

public partial class Program
{
}