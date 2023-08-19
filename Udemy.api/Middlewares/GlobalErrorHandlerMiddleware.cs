using System.Net;
using System.Text.Json;
using Udemy.api.Exceptions;

namespace Udemy.api;

public class GlobalErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            var responseModel = error.Message;

            response.StatusCode = error switch
            {
                ApiException e => (int)HttpStatusCode.BadRequest,// custom application error
                KeyNotFoundException e => (int)HttpStatusCode.NotFound,// not found error
                _ => (int)HttpStatusCode.InternalServerError,// unhandled error
            };
            var result = JsonSerializer.Serialize(responseModel);

            await response.WriteAsync(result);
        }
    }
}