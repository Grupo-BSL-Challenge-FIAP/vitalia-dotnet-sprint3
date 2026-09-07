using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Vitalia.Infrastructure.Data;

namespace Vitalia.API.IntegrationTests;

public sealed class VitaliaWebApplicationFactory
    : WebApplicationFactory<Program>
{
    public const string JwtSecret =
        "vitalia-integration-tests-secret-key-1234567890-abcdef";

    private readonly string _databaseName =
        $"VitaliaIntegrationTests-{Guid.NewGuid()}";

    public VitaliaWebApplicationFactory()
    {
        // Garante que o Program.cs encontre essas configurações
        // antes de montar os serviços da aplicação.
        Environment.SetEnvironmentVariable(
            "JWT_SECRET",
            JwtSecret);

        Environment.SetEnvironmentVariable(
            "ConnectionStrings__OracleConnection",
            "User Id=test;Password=test;Data Source=localhost:1521/TEST;");
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<VitaliaDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["JWT_SECRET"] = JwtSecret,

                    ["ConnectionStrings:OracleConnection"] =
                        "User Id=test;Password=test;Data Source=localhost:1521/TEST;"
                }
            );
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<
                IDbContextOptionsConfiguration<VitaliaDbContext>>();

            services.RemoveAll<
                DbContextOptions<VitaliaDbContext>>();

            services.RemoveAll<VitaliaDbContext>();

            services.AddDbContext<VitaliaDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);

                options.ConfigureWarnings(warnings =>
                    warnings.Ignore(
                        InMemoryEventId.TransactionIgnoredWarning));
            });

            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = "vitalia-api",

                            ValidateAudience = false,

                            ValidateLifetime = true,

                            ValidateIssuerSigningKey = true,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        JwtSecret)
                                ),

                            RoleClaimType = "roles",

                            ClockSkew = TimeSpan.Zero
                        };
                });
        });
    }
}