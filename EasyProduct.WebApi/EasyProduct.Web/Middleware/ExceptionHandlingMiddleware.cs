using EasyProduct.Common.Base;
using EasyProduct.Common.Error;

namespace EasyProduct.Web.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
/// <remarks>
/// 捕获所有未处理的异常，转换为统一的 API 响应格式。
/// 业务异常返回客户端友好的错误信息，系统异常记录日志。
/// </remarks>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="next">下一个中间件</param>
    /// <param name="logger">日志记录器</param>
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    /// <returns>异步任务</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            // 业务异常：不记录日志，直接返回错误响应
            await HandleBusinessExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            // 系统异常：记录日志，返回通用错误
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// 处理业务异常
    /// </summary>
    private async Task HandleBusinessExceptionAsync(HttpContext context, BusinessException exception)
    {
        context.Response.StatusCode = 200;
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Error(exception.Message, exception.Code);
        await context.Response.WriteAsJsonAsync(response);
    }

    /// <summary>
    /// 处理系统异常
    /// </summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "发生未处理的异常");

        context.Response.StatusCode = 200;
        context.Response.ContentType = "application/json";

        var message = "服务器内部错误";
        var response = ApiResponse<object>.Error(message, 500);
        await context.Response.WriteAsJsonAsync(response);
    }
}

/// <summary>
/// 异常处理中间件扩展
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// 使用全局异常处理中间件
    /// </summary>
    /// <param name="app">应用构建器</param>
    /// <returns>应用构建器</returns>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
