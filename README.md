<p align="center">
  <img width="270" height="270" alt="image" src="https://github.com/user-attachments/assets/d72bd46f-639c-4197-94b0-0e3c37791250" />
</p>

# 🐾 Vitalia API - Módulo Comercial

API REST desenvolvida em **ASP.NET Core / .NET 10** para o módulo comercial da plataforma **Vitalia**.

A API é responsável pelo gerenciamento de **categorias, produtos, carrinho de compras, checkout e pedidos**, utilizando **Oracle Database** com **Entity Framework Core**. A autenticação é integrada ao backend Java do Vitalia: o serviço Java emite o JWT e a API .NET valida o token localmente para identificar o usuário e suas permissões.

---

## 📑 Índice

- [👥 Integrantes](#-integrantes)
- [📚 Sobre o Projeto](#-sobre-o-projeto)
- [🏗️ Arquitetura](#️-arquitetura)
- [📁 Estrutura da Solução](#-estrutura-da-solução)
- [🚀 Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [📦 Entidades](#-entidades)
- [🔁 Relacionamentos](#-relacionamentos)
- [👤 Usuários de Avaliação](#-usuários-de-avaliação)
- [🔐 Autenticação e Autorização](#-autenticação-e-autorização)
- [☕ Integração com a API Java](#-integração-com-a-api-java)
- [📡 Endpoints](#-endpoints)
- [📄 HTTP Status Codes](#-http-status-codes)
- [📘 Swagger / OpenAPI](#-swagger--openapi)
- [🩺 Health Checks](#-health-checks)
- [📝 Logs com Serilog](#-logs-com-serilog)
- [📊 OpenTelemetry e Prometheus](#-opentelemetry-e-prometheus)
- [🗄️ Banco de Dados e Migrations](#️-banco-de-dados-e-migrations)
- [⚙️ Como Executar](#️-como-executar)
- [🧪 Testes](#-testes)
- [📈 Cobertura de Testes](#-cobertura-de-testes)
- [🐳 Docker](#-docker)
- [📌 Funcionalidades Implementadas](#-funcionalidades-implementadas)
- [🌐 Ambiente Publicado](#-ambiente-publicado)

---

## 👥 Integrantes

<table>
  <tr>
    <td width="130">
      <img src="https://github.com/moisesBarsoti.png" width="120" style="border-radius: 50%;"/>
    </td>
    <td>
      <b>Moisés Barsoti Andrade de Oliveira</b><br/>
      <b>RM:</b> 565049 &nbsp;&nbsp;|&nbsp;&nbsp;<b>Turma:</b> 2TDSPO - FIAP <br/>
    </td>
  </tr>
  <tr>
    <td width="130">
      <img src="https://github.com/sSofia-s.png" width="120" style="border-radius: 50%;"/>
    </td>
    <td>
      <b>Sofia Siqueira Fontes</b><br/>
      <b>RM:</b> 563829 &nbsp;&nbsp;|&nbsp;&nbsp;<b>Turma:</b> 2TDSPG - FIAP <br/>
    </td>
  </tr>
  <tr>
    <td width="130">
      <img src="https://github.com/manuelalacerda.png" width="120" style="border-radius: 50%;"/>
    </td>
    <td>
      <b>Manuela de Lacerda Soares</b><br/>
      <b>RM:</b> 564887 &nbsp;&nbsp;|&nbsp;&nbsp;<b>Turma:</b> 2TDSPG - FIAP <br/>
    </td>
  </tr>
</table>

---

## 📚 Sobre o Projeto

O **Vitalia** é uma solução voltada ao cuidado e à saúde de pets. Dentro da arquitetura geral da solução, esta API em .NET representa o **módulo comercial**, concentrando as operações de catálogo e compra.

As principais responsabilidades deste serviço são:

- Gerenciamento de categorias;
- Gerenciamento de produtos;
- Controle de estoque;
- Gerenciamento do carrinho do usuário autenticado;
- Inclusão, atualização e remoção de itens do carrinho;
- Checkout;
- Criação de pedidos;
- Atualização do ciclo de vida dos pedidos;
- Persistência no Oracle Database;
- Autenticação JWT integrada ao backend Java;
- Monitoramento, logs, tracing e métricas;
- Testes unitários e de integração.

---

## 🏗️ Arquitetura

O projeto foi estruturado utilizando separação de responsabilidades entre **API, Application, Domain e Infrastructure**.

```txt
Vitalia.API
→ Controllers
→ Configurations
→ Exceptions
→ Extensions
→ Health
→ Swagger / OpenAPI
→ Program.cs

Vitalia.Application
→ DTOs
→ Interfaces
→ Services
→ Repository Contracts

Vitalia.Domain
→ Entities
→ Enums
→ Regras de Negócio

Vitalia.Infrastructure
→ DbContext
→ Configurations
→ Repositories
→ Migrations

Tests
→ Vitalia.Domain.Tests
→ Vitalia.Application.Tests
→ Vitalia.API.IntegrationTests
```

A separação permite manter as regras de negócio desacopladas da persistência, da API HTTP e das ferramentas de infraestrutura.

---

## 📁 Estrutura da Solução

```txt
Vitalia/
│
├── Vitalia.API/
│   ├── Configurations/
│   ├── Controllers/
│   ├── Exceptions/
│   ├── Extensions/
│   ├── Health/
│   ├── Program.cs
│   └── Vitalia.API.csproj
│
├── Vitalia.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Repositories/
│   ├── Services/
│   └── Vitalia.Application.csproj
│
├── Vitalia.Domain/
│   ├── Entities/
│   ├── Enums/
│   └── Vitalia.Domain.csproj
│
├── Vitalia.Infrastructure/
│   ├── Configurations/
│   ├── Migrations/
│   ├── Repositories/
│   └── Vitalia.Infrastructure.csproj
│
└── tests/
    ├── Vitalia.Domain.Tests/
    ├── Vitalia.Application.Tests/
    └── Vitalia.API.IntegrationTests/
```

---

## 🚀 Tecnologias Utilizadas

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- Oracle Database
- Swagger / OpenAPI
- Swashbuckle
- JWT Bearer Authentication
- Serilog
- OpenTelemetry
- Prometheus
- xUnit
- Moq
- WebApplicationFactory
- Coverlet
- Docker
- Repository Pattern
- Service Layer
- Injeção de Dependência

---

## 📦 Entidades

### Category

Representa uma categoria utilizada para organizar os produtos disponíveis no catálogo.

### Product

Representa um produto comercializado pela plataforma, associado a uma categoria e com informações utilizadas no catálogo e no controle de estoque.

### Cart

Representa o carrinho de compras associado ao usuário autenticado.

### CartItem

Representa um produto incluído em um carrinho, armazenando a quantidade selecionada pelo usuário.

### Order

Representa um pedido criado durante o checkout do carrinho.

### OrderItem

Representa um item pertencente a um pedido, preservando os dados necessários para o histórico da compra.

---

## 🔁 Relacionamentos

```txt
Category 1:N Product

Cart 1:N CartItem
Product 1:N CartItem

Order 1:N OrderItem
Product 1:N OrderItem
```

Os carrinhos e pedidos também são associados ao identificador do usuário autenticado recebido através do JWT emitido pelo backend Java do Vitalia.

---

### 👤 Usuários de avaliação

Foram criados usuários específicos para facilitar os testes da integração
entre a API Java e a API .NET.

| Perfil | E-mail |
|---|---|
| TUTOR | `avaliacao@tutor.com.br` |
| ADMIN | `avaliacao@vitaliaadmin.com.br` |

As senhas dos usuários de avaliação são fornecidas separadamente no
arquivo de credenciais da entrega.

> As contas acima foram criadas exclusivamente para demonstração e avaliação acadêmica.

#### Fluxo de teste

1. Acesse o Swagger da API Java:
   `https://vitalia-txa9.onrender.com/swagger-ui/index.html#/`
2. Realize login com um dos usuários de avaliação.
3. Copie o JWT retornado.
4. Acesse o Swagger da API .NET:
   `https://vitalia-dotnet.onrender.com/swagger`
5. Clique em `Authorize`.
6. Informe:

   `Bearer SEU_TOKEN_JWT`

7. Utilize o usuário `TUTOR` para testar carrinho, checkout e pedidos.
8. Utilize o usuário `ADMIN` para testar cadastro de categorias,
   produtos e atualização do ciclo de vida dos pedidos.

---

## 🔐 Autenticação e Autorização

A autenticação dos usuários é responsabilidade do backend Java do Vitalia.

O fluxo utilizado é:

```txt
Usuário
   ↓
API Java / Auth
   ↓
JWT
   ↓
Vitalia.API (.NET)
   ↓
Validação local do token
   ↓
Acesso aos recursos comerciais
```

O token utiliza o identificador do usuário e suas roles para autorização.

Roles reconhecidas pela solução:

```txt
TUTOR
VETERINARIAN
ADMIN
```

As operações administrativas de catálogo e de evolução do status dos pedidos exigem a role `ADMIN`. Operações relacionadas ao carrinho e aos pedidos do próprio usuário exigem autenticação.

No Swagger, utilize o botão **Authorize** e informe:

```txt
Bearer SEU_TOKEN_JWT
```

---

### ☕ Integração com a API Java

A autenticação do Vitalia é centralizada no backend Java.

A API Java é responsável pelo cadastro dos usuários, autenticação e emissão
dos tokens JWT utilizados para acessar os endpoints protegidos da API .NET.

#### Ambiente publicado

Swagger UI:

https://vitalia-txa9.onrender.com/swagger-ui/index.html#/

#### Ambiente local

Swagger UI:

http://localhost:8080/swagger-ui/index.html

#### Backend Java do Vitalia:

https://github.com/Grupo-BSL-Challenge-FIAP/vitalia-JavaAdvanced-Sprint3

O fluxo para testar a integração é:

1. Criar ou utilizar um usuário cadastrado na API Java;
2. Realizar o login pela API Java;
3. Copiar o JWT retornado;
4. Abrir o Swagger da API .NET;
5. Clicar em `Authorize`;
6. Informar:

Bearer SEU_TOKEN_JWT

7. Executar os endpoints comerciais do Vitalia.

---

## 📡 Endpoints

### Category

| Método | Endpoint | Descrição | Acesso |
| --- | --- | --- | --- |
| GET | `/api/Category` | Lista as categorias | Autenticado |
| GET | `/api/Category/{id}` | Busca uma categoria por ID | Autenticado |
| POST | `/api/Category` | Cria uma categoria | ADMIN |
| PUT | `/api/Category/{id}` | Atualiza uma categoria | ADMIN |
| DELETE | `/api/Category/{id}` | Remove uma categoria | ADMIN |

### Product

| Método | Endpoint | Descrição | Acesso |
| --- | --- | --- | --- |
| GET | `/api/Product` | Lista os produtos | Autenticado |
| GET | `/api/Product/paged` | Lista produtos com paginação | Autenticado |
| GET | `/api/Product/{id}` | Busca um produto por ID | Autenticado |
| GET | `/api/Product/category/{categoryId}` | Lista produtos de uma categoria | Autenticado |
| GET | `/api/Product/status/{status}` | Lista produtos por status | Autenticado |
| POST | `/api/Product` | Cria um produto | ADMIN |
| PUT | `/api/Product/{id}` | Atualiza um produto | ADMIN |
| DELETE | `/api/Product/{id}` | Remove um produto | ADMIN |

### Cart

| Método | Endpoint | Descrição | Acesso |
| --- | --- | --- | --- |
| GET | `/api/Cart` | Obtém o carrinho do usuário autenticado | Autenticado |
| GET | `/api/Cart/{id}` | Busca um carrinho por ID | Autenticado |
| POST | `/api/Cart/{cartId}/items` | Adiciona um produto ao carrinho | Autenticado |
| PUT | `/api/Cart/{cartId}/items/{productId}` | Atualiza a quantidade de um produto | Autenticado |
| DELETE | `/api/Cart/{cartId}/items/{productId}` | Remove um produto do carrinho | Autenticado |

### Order

| Método | Endpoint | Descrição | Acesso |
| --- | --- | --- | --- |
| GET | `/api/Order/{id}` | Busca um pedido por ID | Autenticado |
| GET | `/api/Order/my-orders` | Lista os pedidos do usuário autenticado | Autenticado |
| POST | `/api/Order/checkout/{cartId}` | Realiza o checkout do carrinho | Autenticado |
| PUT | `/api/Order/{id}/confirm` | Confirma um pedido | ADMIN |
| PUT | `/api/Order/{id}/process` | Coloca um pedido em processamento | ADMIN |
| PUT | `/api/Order/{id}/ship` | Marca um pedido como enviado | ADMIN |
| PUT | `/api/Order/{id}/deliver` | Marca um pedido como entregue | ADMIN |
| PUT | `/api/Order/{id}/cancel` | Cancela um pedido | Autenticado |

### Ciclo de vida do pedido

```txt
PENDING
   ↓
CONFIRMED
   ↓
PROCESSING
   ↓
SHIPPED
   ↓
DELIVERED
```

Um pedido também pode assumir o status:

```txt
CANCELLED
```

Durante o checkout, a API cria o pedido, registra seus itens, atualiza o estoque e finaliza o carrinho utilizado na compra.

---

## 📄 HTTP Status Codes

| Código | Descrição |
| --- | --- |
| 200 | OK |
| 201 | Created |
| 204 | No Content |
| 400 | Bad Request |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |
| 409 | Conflict |
| 500 | Internal Server Error |

Erros tratados pelo `GlobalExceptionHandler` seguem o padrão **Problem Details (RFC 7807)**.

Exemplo de estrutura:

```json
{
  "type": "https://httpstatuses.com/404",
  "title": "Recurso não encontrado.",
  "status": 404,
  "detail": "Mensagem de erro.",
  "instance": "/api/recurso/1"
}
```

Em ambiente de desenvolvimento, erros tratados também podem incluir o `traceId` para facilitar a correlação com logs e traces.

---

## 📘 Swagger / OpenAPI

A API possui documentação interativa utilizando Swagger / OpenAPI.

Com o perfil HTTP padrão do projeto:

```txt
http://localhost:5230/swagger
```

A documentação permite:

- visualizar os endpoints;
- consultar parâmetros e modelos;
- testar requisições;
- informar JWT Bearer pelo botão **Authorize**;
- visualizar os principais códigos de resposta HTTP.

---

## 🩺 Health Checks

A aplicação possui endpoints de monitoramento de saúde.

| Endpoint | Função |
| --- | --- |
| `/health` | Retorna a saúde geral da aplicação e suas dependências registradas |
| `/health/live` | Verifica se a API está em execução |
| `/health/ready` | Verifica se a aplicação está pronta para receber tráfego |

O endpoint de readiness valida também a conectividade com o **Oracle Database**.

Exemplo:

```bash
curl http://localhost:5230/health
```

Resposta esperada quando os serviços estão saudáveis:

```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "Oracle",
      "status": "Healthy"
    }
  ]
}
```

---

## 📝 Logs com Serilog

A aplicação utiliza **Serilog** para logging estruturado.

As requisições HTTP registram informações como:

- método HTTP;
- rota;
- status code;
- tempo de resposta;
- `TraceId`;
- `RequestId`.

Exemplo:

```txt
HTTP GET /health responded 200 in 10.1585 ms |
TraceId: 123385a09cdd9f5389dcd9145f88fada |
RequestId: 0HNOD3VN7J5U7
```

O projeto utiliza níveis de logging como:

```txt
Information
Warning
Error
```

O `TraceId` permite correlacionar a requisição registrada pelo Serilog com o trace criado pelo OpenTelemetry.

---

## 📊 OpenTelemetry e Prometheus

A API utiliza **OpenTelemetry** para observabilidade.

### Tracing

São instrumentados:

- ASP.NET Core;
- requisições HTTP realizadas por `HttpClient`;
- Entity Framework Core.

Os traces incluem informações como:

```txt
TraceId
SpanId
Duration
HTTP Method
Route
Status Code
Service Name
```

O serviço é identificado como:

```txt
Vitalia.API
```

### Métricas

As métricas são disponibilizadas para Prometheus através de:

```txt
/metrics
```

Exemplo:

```bash
curl http://localhost:5230/metrics
```

Entre as métricas coletadas estão dados de:

- requisições HTTP;
- duração das requisições;
- runtime do .NET;
- conexões e servidor ASP.NET Core;
- chamadas HTTP externas.

Essas métricas podem ser utilizadas para acompanhar tempo de resposta, volume de requisições e taxa de erros.

---

## 🗄️ Banco de Dados e Migrations

A aplicação utiliza **Oracle Database** com **Entity Framework Core**.

O `VitaliaDbContext` gerencia as tabelas comerciais relacionadas a:

```txt
Category
Product
Cart
CartItem
Order
OrderItem
```

O projeto possui uma migration inicial versionada:

```txt
20260907194842_InitialCommercialSchema
```

Arquivos principais:

```txt
Vitalia.Infrastructure/Migrations/
├── 20260907194842_InitialCommercialSchema.cs
├── 20260907194842_InitialCommercialSchema.Designer.cs
└── VitaliaDbContextModelSnapshot.cs
```

Para listar migrations:

```bash
dotnet ef migrations list \
  --project Vitalia.Infrastructure/Vitalia.Infrastructure.csproj \
  --startup-project Vitalia.API/Vitalia.API.csproj \
  --context VitaliaDbContext
```

Em um schema Oracle novo, a migration pode ser aplicada com:

```bash
dotnet ef database update \
  --project Vitalia.Infrastructure/Vitalia.Infrastructure.csproj \
  --startup-project Vitalia.API/Vitalia.API.csproj \
  --context VitaliaDbContext
```

> **Atenção:** caso o schema Oracle já possua as tabelas criadas, verifique o estado do banco antes de executar `database update` para evitar conflito com objetos já existentes.

---

## ⚙️ Como Executar

### 1. Clonar o repositório

```bash
git clone https://github.com/Grupo-BSL-Challenge-FIAP/vitalia-dotnet-sprint3
```

### 2. Entrar na pasta do projeto

```bash
cd vitalia-dotnet-sprint3
```

### 3. Restaurar as dependências

```bash
dotnet restore
```

### 4. Configurar os segredos

Inicialize o User Secrets, caso ainda não esteja configurado:

```bash
dotnet user-secrets init \
  --project Vitalia.API/Vitalia.API.csproj
```

Configure a conexão Oracle:

```bash
dotnet user-secrets set \
  "ConnectionStrings:OracleConnection" \
  "User Id=USUARIO;Password=SENHA;Data Source=oracle.fiap.com.br:1521/ORCL;" \
  --project Vitalia.API/Vitalia.API.csproj
```

Configure o segredo utilizado na validação do JWT:

```bash
dotnet user-secrets set \
  "JWT_SECRET" \
  "JWT_CREDENCIAL" \
  --project Vitalia.API/Vitalia.API.csproj
```

> O valor do segredo JWT não deve ser versionado no Git. A API .NET precisa utilizar o segredo compatível com o token emitido pelo backend Java do Vitalia.

### 5. Compilar

```bash
dotnet build
```

### 6. Executar a API

```bash
dotnet run \
  --project Vitalia.API/ \
  --launch-profile http
```

A aplicação será disponibilizada em:

```txt
http://localhost:5230
```

Swagger:

```txt
http://localhost:5230/swagger
```

---

## 🧪 Testes

Os testes estão separados em três projetos:

```txt
tests/
├── Vitalia.Domain.Tests
├── Vitalia.Application.Tests
└── Vitalia.API.IntegrationTests
```

### Testes unitários

Os testes de domínio e aplicação utilizam:

- xUnit;
- Moq;
- padrão Arrange / Act / Assert;
- nomes de testes descritivos seguindo o cenário e o resultado esperado.

### Testes de integração

Os testes de integração utilizam:

- `WebApplicationFactory`;
- `IClassFixture`;
- banco em memória para isolamento;
- autenticação JWT de teste;
- validação de cenários de sucesso, autenticação e erros HTTP.

Para executar toda a suíte:

```bash
dotnet test
```

Resultado atual da suíte:

```txt
Total: 69
Sucesso: 69
Falhas: 0
Ignorados: 0
```

---

## 📈 Cobertura de Testes

Para gerar os relatórios de cobertura:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Os relatórios são gerados dentro das pastas `TestResults` de cada projeto de teste.

Cobertura de linhas observada nos relatórios individuais:

| Projeto | Cobertura de linhas |
| --- | ---: |
| Vitalia.Domain.Tests | 22,99% |
| Vitalia.Application.Tests | 73,39% |
| Vitalia.API.IntegrationTests | 41,99% |

A consolidação das linhas únicas presentes nos relatórios Cobertura resultou em aproximadamente:

```txt
45,28% de cobertura de linhas
```

> A porcentagem consolidada foi calculada a partir dos relatórios Cobertura gerados pelos projetos de teste e não representa um merge oficial do Coverlet.

---

## 🐳 Docker

O projeto possui um `Dockerfile` para publicação da API.

A partir da raiz da solução:

```bash
docker build \
  -f Vitalia.API/Dockerfile \
  -t vitalia-api .
```

Para executar o container:

```bash
docker run --rm \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__OracleConnection="<SUA_CONNECTION_STRING>" \
  -e JWT_SECRET="<SEU_SEGREDO_JWT>" \
  vitalia-api
```

A API ficará disponível em:

```txt
http://localhost:8080
```

---

## 📌 Funcionalidades Implementadas

- CRUD de categorias;
- CRUD de produtos;
- rotas parametrizadas;
- paginação de produtos;
- filtros por categoria e status;
- gerenciamento de carrinho;
- adição, atualização e remoção de itens;
- checkout;
- criação de pedidos;
- controle de estoque durante o checkout;
- ciclo de vida dos pedidos;
- autenticação JWT;
- autorização por roles;
- Oracle Database;
- Entity Framework Core;
- migrations;
- Repository Pattern;
- Service Layer;
- injeção de dependência;
- tratamento global de exceções;
- Problem Details;
- Swagger / OpenAPI;
- Health Checks;
- Serilog;
- correlação por `TraceId` e `RequestId`;
- OpenTelemetry;
- métricas Prometheus;
- testes unitários;
- testes de integração;
- fixtures com xUnit;
- cobertura com Coverlet;
- Docker.

---

## 🌐 Ambiente Publicado

A API do módulo comercial do Vitalia está publicada no **Render** e pode ser acessada pelos links abaixo.

| Recurso | URL |
|---|---|
| 🚀 **API .NET** | https://vitalia-dotnet.onrender.com |
| 📘 **Swagger / OpenAPI** | https://vitalia-dotnet.onrender.com/swagger |
| 🩺 **Health Check** | https://vitalia-dotnet.onrender.com/health |
| 💚 **Liveness** | https://vitalia-dotnet.onrender.com/health/live |
| ✅ **Readiness** | https://vitalia-dotnet.onrender.com/health/ready |
| 📊 **Métricas Prometheus** | https://vitalia-dotnet.onrender.com/metrics |

> ℹ️ O serviço está hospedado no plano gratuito do Render. Após um período de inatividade, a primeira requisição pode levar alguns segundos para responder enquanto a instância é reativada.
