using System.Diagnostics;

namespace EasyProduct.Web.Middleware;

/// <summary>
/// 请求日志中间件
/// </summary>
/// <remarks>
/// 记录每个请求的进入和完成信息，包括请求路径、方法、耗时等。
/// </remarks>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="next">下一个中间件</param>
    /// <param name="logger">日志记录器</param>
    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
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
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;

        _logger.LogInformation(
            "[Request] {Method} {Path}{QueryString} - 开始处理",
            request.Method,
            request.Path,
            request.QueryString);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;

            _logger.LogInformation(
                "[Request] {Method} {Path} - 完成，状态码：{StatusCode}，耗时：{ElapsedMs}ms",
                request.Method,
                request.Path,
                statusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}

/// <summary>
/// 请求日志中间件扩展
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// 使用请求日志中间件
    /// </summary>
    /// <param name="app">应用构建器</param>
    /// <returns>应用构建器</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
