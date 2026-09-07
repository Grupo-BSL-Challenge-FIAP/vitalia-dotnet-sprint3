using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Vitalia.API.Configurations;

public static class JwtConfiguration
{
    public const string Issuer = "vitalia-api";

    public static void Configure(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSecret = configuration["JWT_SECRET"]
                        ?? throw new InvalidOperationException(
                            "A variável JWT_SECRET não foi encontrada."
                        );

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,

                    ValidateAudience = false,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSecret)
                    ),

                    RoleClaimType = "roles"
                };
            });

        services.AddAuthorization();
    }
}