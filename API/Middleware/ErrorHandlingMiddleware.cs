using System;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API.Middleware
{
  public class ErrorHandlingMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> logger;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
    {
      this._next = next;
      this.logger = logger;
    }

    public async Task Invoke(HttpContext context) {
        try
        {
            await _next(context);
        }
        catch (System.Exception ex)
        {
            await HandleExceptionAsync(context, ex, logger);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context,
        Exception ex, ILogger<ErrorHandlingMiddleware> logger)
    {
        object errors = null;

        switch (ex) {
            case RestException re:
                logger.LogError(ex, "REST ERROR");
                errors = re.Errors;
                context.Response.StatusCode = (int)re.Code;
                break;

            case Exception e:
                logger.LogError(ex, "SERVER ERROR");
                errors = string.IsNullOrWhiteSpace(e.Message)
                    ? "Error"
                    : e.Message;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.ContentType = "application/json";
        if (errors != null)
        {
            var result = JsonConvert.SerializeObject(new {
                errors
            });

            await context.Response.WriteAsync(result);
        }
    }
  }
}