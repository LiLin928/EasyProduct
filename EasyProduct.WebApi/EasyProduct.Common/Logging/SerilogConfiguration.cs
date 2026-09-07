using Serilog;
using Serilog.Events;

namespace EasyProduct.Common.Logging;

/// <summary>
/// Serilog 配置类
/// </summary>
/// <remarks>
/// 提供日志配置的静态方法，支持控制台和文件输出。
/// </remarks>
public static class SerilogConfiguration
{
    /// <summary>
    /// 配置 Serilog 日志
    /// </summary>
    /// <remarks>
    /// 配置日志输出到控制台和滚动文件。
    /// 日志级别：Information 及以上写入文件，Debug 及以上输出到控制台。
    /// </remarks>
    public static void Configure()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] {Message}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] {Message}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information)
            .CreateLogger();
    }
}
