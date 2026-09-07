using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;

namespace Vitalia.API.IntegrationTests;

public class ApiIntegrationTests : IDisposable
{
    private readonly VitaliaWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ApiIntegrationTests()
    {
        _factory = new VitaliaWebApplicationFactory();

        _client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
    }

    [Fact]
    public async Task Swagger_EmDevelopment_DeveRetornar200()
    {
        var response =
            await _client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task Health_DeveRetornarHealthy()
    {
        var response =
            await _client.GetAsync("/health");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var body =
            await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "Healthy",
            body);
    }
    [Fact]
    public async Task HealthLive_DeveRetornarHealthy()
    {
        var response =
            await _client.GetAsync("/health/live");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var body =
            await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "Healthy",
            body);
    }

    [Fact]
    public async Task HealthReady_DeveRetornarHealthy()
    {
        var response =
            await _client.GetAsync("/health/ready");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var body =
            await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "Healthy",
            body);

        Assert.Contains(
            "Oracle",
            body);
    }

    [Fact]
    public async Task RotaInexistente_DeveRetornar404()
    {
        var response =
            await _client.GetAsync(
                "/api/rota-que-nao-existe");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task BuscarProdutos_SemJwt_DeveRetornar401()
    {
        var response =
            await _client.GetAsync("/api/Product");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task BuscarProdutos_JwtInvalido_DeveRetornar401()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                "token.invalido.vitalia");

        var response =
            await _client.GetAsync("/api/Product");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task BuscarProdutos_JwtExpirado_DeveRetornar401()
    {
        var token = CreateToken(
            userId: 10,
            roles: ["TUTOR"],
            expiresAt: DateTime.UtcNow.AddMinutes(-10));

        Authenticate(token);

        var response =
            await _client.GetAsync("/api/Product");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task CriarProduto_Tutor_DeveRetornar403()
    {
        Authenticate(
            CreateToken(
                userId: 10,
                roles: ["TUTOR"]));

        var request = new
        {
            categoryId = 1,
            name = "Produto Teste",
            description = "Produto teste",
            price = 50m,
            stock = 10
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/Product",
                request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task BuscarProdutos_TutorAutenticado_DeveRetornar200()
    {
        Authenticate(
            CreateToken(
                userId: 10,
                roles: ["TUTOR"]));

        var response =
            await _client.GetAsync("/api/Product");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task BuscarProdutos_PaginaInvalida_DeveRetornar400()
    {
        Authenticate(
            CreateToken(
                userId: 10,
                roles: ["TUTOR"]));

        var response =
            await _client.GetAsync(
                "/api/Product/paged?page=0&pageSize=10");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task BuscarProduto_Inexistente_DeveRetornar404()
    {
        Authenticate(
            CreateToken(
                userId: 10,
                roles: ["TUTOR"]));

        var response =
            await _client.GetAsync(
                "/api/Product/999999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task CriarProduto_DadosInvalidos_DeveRetornar400()
    {
        Authenticate(
            CreateToken(
                userId: 1,
                roles: ["ADMIN"]));

        var request = new
        {
            categoryId = 0,
            name = "",
            description = "Inválido",
            price = 0,
            stock = -1
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/Product",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task CrudProduto_Admin_DeveCriarConsultarAtualizarEExcluir()
    {
        // Arrange
        Authenticate(
            CreateToken(
                userId: 1,
                roles: ["ADMIN"]));

        var categoryId =
            await CreateCategoryAsync();

        // CREATE
        var createRequest = new
        {
            categoryId,
            name = "Ração Integration",
            description = "Produto criado pelo teste",
            price = 100m,
            stock = 20
        };

        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/Product",
                createRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var createdJson =
            await ReadJsonAsync(createResponse);

        var productId =
            createdJson.GetProperty("id").GetInt64();

        // READ
        var getResponse =
            await _client.GetAsync(
                $"/api/Product/{productId}");

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        // UPDATE
        var updateRequest = new
        {
            categoryId,
            name = "Ração Integration Atualizada",
            description = "Produto atualizado",
            price = 120m,
            stock = 30
        };

        var updateResponse =
            await _client.PutAsJsonAsync(
                $"/api/Product/{productId}",
                updateRequest);

        Assert.Equal(
            HttpStatusCode.NoContent,
            updateResponse.StatusCode);

        var updatedResponse =
            await _client.GetAsync(
                $"/api/Product/{productId}");

        var updatedJson =
            await ReadJsonAsync(updatedResponse);

        Assert.Equal(
            "Ração Integration Atualizada",
            updatedJson
                .GetProperty("name")
                .GetString());

        Assert.Equal(
            120m,
            updatedJson
                .GetProperty("price")
                .GetDecimal());

        Assert.Equal(
            30,
            updatedJson
                .GetProperty("stock")
                .GetInt32());

        // DELETE
        var deleteResponse =
            await _client.DeleteAsync(
                $"/api/Product/{productId}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        var afterDelete =
            await _client.GetAsync(
                $"/api/Product/{productId}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            afterDelete.StatusCode);
    }

    [Fact]
    public async Task FluxoCompra_DeveCriarCarrinhoAdicionarProdutoCheckoutEGerenciarPedido()
    {

        Authenticate(
            CreateToken(
                userId: 1,
                roles: ["ADMIN"]));

        var categoryId =
            await CreateCategoryAsync();

        var productId =
            await CreateProductAsync(
                categoryId,
                price: 50m,
                stock: 10);

        const long tutorUserId = 42;

        Authenticate(
            CreateToken(
                tutorUserId,
                ["TUTOR"]));

        // Cria / obtém carrinho
        var cartResponse =
            await _client.GetAsync("/api/Cart");

        Assert.Equal(
            HttpStatusCode.OK,
            cartResponse.StatusCode);

        var cartJson =
            await ReadJsonAsync(cartResponse);

        var cartId =
            cartJson.GetProperty("id").GetInt64();

        Assert.Equal(
            tutorUserId,
            cartJson
                .GetProperty("userId")
                .GetInt64());

        // Adiciona 2 unidades
        var addItemResponse =
            await _client.PostAsJsonAsync(
                $"/api/Cart/{cartId}/items",
                new
                {
                    productId,
                    quantity = 2
                });

        Assert.Equal(
            HttpStatusCode.OK,
            addItemResponse.StatusCode);

        // Atualiza para 3 unidades
        var updateItemResponse =
            await _client.PutAsJsonAsync(
                $"/api/Cart/{cartId}/items/{productId}",
                3);

        Assert.Equal(
            HttpStatusCode.OK,
            updateItemResponse.StatusCode);

        var updatedCartJson =
            await ReadJsonAsync(updateItemResponse);

        Assert.Equal(
            150m,
            updatedCartJson
                .GetProperty("total")
                .GetDecimal());

        var checkoutResponse =
            await _client.PostAsync(
                $"/api/Order/checkout/{cartId}",
                null);

        Assert.Equal(
            HttpStatusCode.OK,
            checkoutResponse.StatusCode);

        var orderJson =
            await ReadJsonAsync(checkoutResponse);

        var orderId =
            orderJson.GetProperty("id").GetInt64();

        Assert.Equal(
            tutorUserId,
            orderJson
                .GetProperty("userId")
                .GetInt64());

        Assert.Equal(
            150m,
            orderJson
                .GetProperty("totalAmount")
                .GetDecimal());

        var myOrdersResponse =
            await _client.GetAsync(
                "/api/Order/my-orders");

        Assert.Equal(
            HttpStatusCode.OK,
            myOrdersResponse.StatusCode);

        var myOrdersJson =
            await ReadJsonAsync(myOrdersResponse);

        Assert.True(
            myOrdersJson.GetArrayLength() >= 1);

        var ownOrderResponse =
            await _client.GetAsync(
                $"/api/Order/{orderId}");

        Assert.Equal(
            HttpStatusCode.OK,
            ownOrderResponse.StatusCode);


        var productResponse =
            await _client.GetAsync(
                $"/api/Product/{productId}");

        Assert.Equal(
            HttpStatusCode.OK,
            productResponse.StatusCode);

        var productJson =
            await ReadJsonAsync(productResponse);

        Assert.Equal(
            7,
            productJson
                .GetProperty("stock")
                .GetInt32());
        

        Authenticate(
            CreateToken(
                userId: 1,
                roles: ["ADMIN"]));

        var confirmResponse =
            await _client.PutAsync(
                $"/api/Order/{orderId}/confirm",
                null);

        Assert.Equal(
            HttpStatusCode.NoContent,
            confirmResponse.StatusCode);

        var processResponse =
            await _client.PutAsync(
                $"/api/Order/{orderId}/process",
                null);

        Assert.Equal(
            HttpStatusCode.NoContent,
            processResponse.StatusCode);

        var shipResponse =
            await _client.PutAsync(
                $"/api/Order/{orderId}/ship",
                null);

        Assert.Equal(
            HttpStatusCode.NoContent,
            shipResponse.StatusCode);

        var deliverResponse =
            await _client.PutAsync(
                $"/api/Order/{orderId}/deliver",
                null);

        Assert.Equal(
            HttpStatusCode.NoContent,
            deliverResponse.StatusCode);
        

        Authenticate(
            CreateToken(
                tutorUserId,
                ["TUTOR"]));

        var finalOrderResponse =
            await _client.GetAsync(
                $"/api/Order/{orderId}");

        Assert.Equal(
            HttpStatusCode.OK,
            finalOrderResponse.StatusCode);

        var finalOrderJson =
            await ReadJsonAsync(finalOrderResponse);

        var status =
            finalOrderJson.GetProperty("status");

        var delivered =
            status.ValueKind switch
            {
                JsonValueKind.String =>
                    status.GetString() == "DELIVERED",

                JsonValueKind.Number =>
                    status.GetInt32() == 4,

                _ => false
            };

        Assert.True(delivered);
    }

    private async Task<long> CreateCategoryAsync()
    {
        var response =
            await _client.PostAsJsonAsync(
                "/api/Category",
                new
                {
                    name = "Categoria Integration",
                    description =
                        "Categoria criada durante teste"
                });

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var json =
            await ReadJsonAsync(response);

        return json
            .GetProperty("id")
            .GetInt64();
    }

    private async Task<long> CreateProductAsync(
        long categoryId,
        decimal price,
        int stock)
    {
        var response =
            await _client.PostAsJsonAsync(
                "/api/Product",
                new
                {
                    categoryId,
                    name = "Produto Integration",
                    description =
                        "Produto criado durante teste",
                    price,
                    stock
                });

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var json =
            await ReadJsonAsync(response);

        return json
            .GetProperty("id")
            .GetInt64();
    }

    private void Authenticate(string token)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);
    }

    private static string CreateToken(
        long userId,
        string[] roles,
        DateTime? expiresAt = null)
    {
        var now =
            DateTime.UtcNow;

        var expiration =
            expiresAt ?? now.AddHours(1);

        var notBefore =
            expiration <= now
                ? expiration.AddHours(-1)
                : now.AddMinutes(-1);

        var claims =
            new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Sub,
                    "integration@vitalia.test"),

                new(
                    "userId",
                    userId.ToString())
            };

        claims.AddRange(
            roles.Select(role =>
                new Claim("roles", role)));

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    VitaliaWebApplicationFactory.JwtSecret));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: "vitalia-api",
                audience: null,
                claims: claims,
                notBefore: notBefore,
                expires: expiration,
                signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    private static async Task<JsonElement> ReadJsonAsync(
        HttpResponseMessage response)
    {
        var body =
            await response.Content
                .ReadAsStringAsync();

        using var document =
            JsonDocument.Parse(body);

        return document.RootElement.Clone();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}