# **Pokémon Wrapper API (.NET 8)**

This project is a .NET 8 ASP.NET Core Web API that acts as a secure wrapper over PokeAPI.
It demonstrates clean architecture, authentication, resilience, caching, logging best practices.

-----------------------------------------------

## Features

- ASP.NET Core Web API (.NET 8)

- Typed HttpClient with HttpClientFactory

- JWT Authentication + Refresh Tokens

- Refresh token rotation & revocation

- HttpOnly cookie–based refresh tokens

- Centralized exception handling

- IMemoryCache

- Polly retry & circuit breaker

- Structured logging with ILogger

--------------------------------------------------

## Prerequisites

Make sure you have the following installed:

- .NET 8 SDK

- Git

- Any IDE (Visual Studio / VS Code / Rider)

Verify installation:

```
dotnet --version
```

## How to Run the Project

- Clone the repository

```
git clone https://github.com/arsh-9/PokeAPIService.git
cd PokemonApi
```

- Restore dependencies

```
dotnet restore
```

- Run the API

```
dotnet run
```