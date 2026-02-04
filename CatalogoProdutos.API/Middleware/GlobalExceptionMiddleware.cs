using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CatalogoProdutos.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next, 
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu uma exceção não tratada: {Message}", ex.Message);
                _logger.LogError(ex, "Stack Trace: {StackTrace}", ex.StackTrace);
                
                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Erro interno no servidor",
                    Detail = _environment.IsDevelopment() 
                        ? $"Erro: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}"
                        : "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."
                };

                // Adiciona detalhes extras em desenvolvimento
                if (_environment.IsDevelopment())
                {
                    problemDetails.Extensions.Add("exceptionType", ex.GetType().Name);
                    problemDetails.Extensions.Add("innerException", ex.InnerException?.Message);
                }

                var responseJson = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                
                await context.Response.WriteAsync(responseJson);
            }
        }
    }
}
