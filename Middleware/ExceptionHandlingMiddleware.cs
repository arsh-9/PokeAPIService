namespace PokeAPIService.Middleware;

using System.Net;
using System.Text.Json;
using PokeAPIService.Exceptions;
using PokeAPIService.Models;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.BadGateway, ex.Message);
        }
        catch (Exception ex)
        {
            await WriteErrorAsync(
                context,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var error = new ApiError
        {
            StatusCode = context.Response.StatusCode,
            Message = message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(error));
    }
}
