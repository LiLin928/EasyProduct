using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using SqlSugar;

namespace EasyProduct.Common.Extensions;

/// <summary>
/// SqlSugar 扩展方法
/// </summary>
public static class SqlSugarExtensions
{
    /// <summary>
    /// 添加 SqlSugar 服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置对象</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddSqlSugarService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("数据库连接字符串配置错误");
        }

        // 使用 SqlSugar 单例模式
        var db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = connectionString,
            DbType = DbType.MySqlConnector,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute,
            // 设置默认映射：实体 PascalCase -> 表名 snake_case
            ConfigureExternalServices = new ConfigureExternalServices()
            {
                EntityService = (type, entity) =>
                {
                    // 自动将 PascalCase 转换为 snake_case
                    entity.DbTableName = ConvertToSnakeCase(entity.DbTableName);
                }
            }
        });

        // 启用 AOP 日志（开发环境）
        var isDevelopment = configuration.GetValue<bool>("IsDevelopment");
        if (isDevelopment)
        {
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine($"[SQL] {sql}");
            };
        }

        services.AddSingleton<ISqlSugarClient>(db);
        services.AddSingleton(db); // 同时注册 SqlSugarClient 实例

        return services;
    }

    /// <summary>
    /// 将 PascalCase 转换为 snake_case
    /// </summary>
    private static string ConvertToSnakeCase(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var result = new System.Text.StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]) && i > 0)
            {
                result.Append('_');
            }
            result.Append(char.ToLower(name[i]));
        }
        return result.ToString();
    }
}
