using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.IdentityModel.Tokens;

using PokeAPIService.Clients;

using PokeAPIService.Middleware;

using PokeAPIService.Services;

using Polly;

using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMemoryCache();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

    .AddJwtBearer(options =>

    {

        options.TokenValidationParameters = new TokenValidationParameters

        {

            ValidateAudience = true,

            ValidateIssuer = true,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            ValidIssuer = "PokemonApi",

            ValidAudience = "PokemonApiUsers",

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("qwertyuioplkjhgfdsazxcvbnmqwertyuiop"))

        };

    });

builder.Services.AddAuthorization();

builder.Services.AddHttpClient<IPokeApiClient, PokeApiClient>(client =>

{

    client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");

    client.Timeout = TimeSpan.FromSeconds(10);

}).AddPolicyHandler(GetRetryPolicy()).AddPolicyHandler(GetCircuitBreakerPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()

{

    return HttpPolicyExtensions.HandleTransientHttpError()

        .WaitAndRetryAsync(

        retryCount: 3,

        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),

        onRetry: (outcome, timespan, retryAttempt, context) => { Console.WriteLine("retry"); }

        );

}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()

{

    return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(

        handledEventsAllowedBeforeBreaking: 5, durationOfBreak: TimeSpan.FromSeconds(30),

        onBreak: (outcome, timespan) => { Console.WriteLine("break"); },

        onReset: () => { Console.WriteLine("reset"); }

        );

}


builder.Services.AddScoped<IPokemonService, PokemonService>();
builder.Services.AddSingleton<IRefreshTokenStore, RefreshTokenStore>();
builder.Services.AddScoped<ITokenService, TokenService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())

{

    app.UseSwagger();

    app.UseSwaggerUI();

}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();