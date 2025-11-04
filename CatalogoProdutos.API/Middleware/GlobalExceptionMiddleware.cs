using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CatalogoProdutos.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)

        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu uma exceção não tratada: {ex.Message}");
                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var problemDetails = new ProblemDetails

                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Erro interno no servidor",
                    Detail = "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."
                };

                var responseJson = JsonSerializer.Serialize(problemDetails);
                context.Response.WriteAsync(responseJson);
               

            }
        }
    }
}
