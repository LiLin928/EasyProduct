using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Constants;
using EasyProduct.Models.Dto.Report.Datasource;
using EasyProduct.Models.Entitys.Report;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Report;

/// <summary>
/// 数据源服务实现
/// </summary>
/// <remarks>
/// 提供数据源的增删改查、连接测试等功能
/// 继承 BaseService，使用属性注入获取数据库上下文
/// </remarks>
public class RptDatasourceService : BaseService, IRptDatasourceService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<RptDatasourceService> _logger;

    /// <summary>
    /// 构造函数，通过依赖注入获取日志记录器
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public RptDatasourceService(ILogger<RptDatasourceService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取数据源分页列表
    /// </summary>
    /// <param name="query">查询参数，包含分页、名称、类型、状态等筛选条件</param>
    /// <returns>数据源分页列表结果</returns>
    /// <remarks>
    /// 1. 支持按名称模糊搜索
    /// 2. 支持按数据源类型筛选
    /// 3. 支持按连接状态筛选
    /// 4. 默认按创建时间倒序排列
    /// 5. 密码字段脱敏显示
    /// </remarks>
    public async Task<PageResponse<RptDatasourceDto>> GetListAsync(RptDatasourceQuery query)
    {
        // 1. 构建查询条件
        var whereExpr = Expressionable.Create<RptDatasource>()
            .AndIF(!string.IsNullOrEmpty(query.Name), d => d.Name.Contains(query.Name!))
            .AndIF(!string.IsNullOrEmpty(query.Type), d => d.Type == query.Type)
            .AndIF(query.Status.HasValue, d => d.Status == query.Status!.Value)
            .ToExpression();

        // 2. 分页查询
        var queryable = _db.Queryable<RptDatasource>()
            .Where(whereExpr)
            .OrderByDescending(d => d.CreatedAt);

        RefAsync<int> total = 0;
        var datasources = await queryable
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 3. 转换为 DTO 并脱敏密码
        var datasourceDtos = datasources.Adapt<List<RptDatasourceDto>>();
        for (int i = 0; i < datasourceDtos.Count; i++)
        {
            datasourceDtos[i].Password = "******";
            datasourceDtos[i].CreateTime = datasources[i].CreatedAt;
            datasourceDtos[i].UpdateTime = datasources[i].UpdatedAt ?? DateTime.MinValue;
        }

        // 4. 返回分页结果
        return PageResponse<RptDatasourceDto>.Create(datasourceDtos, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 根据ID获取数据源详情
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <returns>数据源详情信息，如果不存在则返回 null</returns>
    /// <remarks>
    /// 1. 根据ID查询数据源
    /// 2. 不存在时抛出业务异常
    /// 3. 密码字段脱敏显示
    /// </remarks>
    public async Task<RptDatasourceDto?> GetByIdAsync(Guid id)
    {
        // 1. 查询数据源
        var datasource = await _db.Queryable<RptDatasource>()
            .Where(d => d.Id == id)
            .FirstAsync();

        // 2. 检查数据源是否存在
        if (datasource == null)
        {
            throw BusinessException.NotFound("数据源不存在");
        }

        // 3. 转换为 DTO 并脱敏密码
        var dto = datasource.Adapt<RptDatasourceDto>();
        dto.Password = "******";

        return dto;
    }

    /// <summary>
    /// 创建数据源
    /// </summary>
    /// <param name="dto">创建数据源参数</param>
    /// <returns>创建成功返回新记录的ID</returns>
    /// <remarks>
    /// 1. 验证数据源名称唯一性
    /// 2. 密码进行加密存储（Base64编码）
    /// 3. 默认连接状态为错误
    /// 4. 创建数据源记录
    /// </remarks>
    public async Task<Guid> CreateAsync(RptDatasourceCreateDto dto)
    {
        // 1. 检查数据源名称是否已存在
        var exists = await _db.Queryable<RptDatasource>()
            .Where(d => d.Name == dto.Name)
            .AnyAsync();

        if (exists)
        {
            throw BusinessException.BadRequest("数据源名称已存在");
        }

        // 2. 创建数据源实体
        var datasource = dto.Adapt<RptDatasource>();
        datasource.Id = Guid.NewGuid();
        datasource.Password = EncryptPassword(dto.Password);
        datasource.Status = ReportConstants.DatasourceStatus.Error;
        datasource.CreatedAt = DateTime.UtcNow;

        // 3. 插入数据库
        await _db.Insertable(datasource).ExecuteCommandAsync();

        _logger.LogInformation("创建数据源成功: {Name}", datasource.Name);

        return datasource.Id;
    }

    /// <summary>
    /// 更新数据源
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <param name="dto">更新数据源参数</param>
    /// <returns>更新成功返回 true，失败返回 false</returns>
    /// <remarks>
    /// 1. 验证数据源是否存在
    /// 2. 验证数据源名称唯一性（排除自己）
    /// 3. 如果密码为空则不修改密码
    /// 4. 如果修改了连接参数，重置连接状态为错误
    /// 5. 更新数据源记录
    /// </remarks>
    public async Task<bool> UpdateAsync(Guid id, RptDatasourceUpdateDto dto)
    {
        // 1. 查询数据源
        var datasource = await _db.Queryable<RptDatasource>()
            .Where(d => d.Id == id)
            .FirstAsync();

        if (datasource == null)
        {
            throw BusinessException.NotFound("数据源不存在");
        }

        // 2. 检查数据源名称是否已存在（排除自己）
        var exists = await _db.Queryable<RptDatasource>()
            .Where(d => d.Name == dto.Name && d.Id != id)
            .AnyAsync();

        if (exists)
        {
            throw BusinessException.BadRequest("数据源名称已存在");
        }

        // 3. 更新字段
        datasource.Name = dto.Name;
        datasource.Type = dto.Type;
        datasource.Host = dto.Host;
        datasource.Port = dto.Port;
        datasource.Database = dto.Database;
        datasource.Username = dto.Username;
        datasource.Remark = dto.Remark;
        datasource.UpdatedAt = DateTime.UtcNow;

        // 4. 如果密码不为空，则更新密码
        if (!string.IsNullOrEmpty(dto.Password))
        {
            datasource.Password = EncryptPassword(dto.Password);
        }

        // 5. 如果修改了连接参数，重置连接状态
        if (datasource.Host != dto.Host ||
            datasource.Port != dto.Port ||
            datasource.Database != dto.Database ||
            datasource.Username != dto.Username ||
            !string.IsNullOrEmpty(dto.Password))
        {
            datasource.Status = ReportConstants.DatasourceStatus.Error;
            datasource.LastTestTime = null;
            datasource.ErrorMessage = null;
        }

        // 6. 更新数据库
        var result = await _db.Updateable(datasource).ExecuteCommandAsync();

        if (result > 0)
        {
            _logger.LogInformation("更新数据源成功: {Name}", datasource.Name);
        }

        return result > 0;
    }

    /// <summary>
    /// 删除数据源
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <returns>删除成功返回 true，失败返回 false</returns>
    /// <remarks>
    /// 删除前需要检查该数据源是否被报表引用，如果被引用则不允许删除
    /// </remarks>
    public async Task<bool> DeleteAsync(Guid id)
    {
        // 1. 查询数据源
        var datasource = await _db.Queryable<RptDatasource>()
            .Where(d => d.Id == id)
            .FirstAsync();

        if (datasource == null)
        {
            throw BusinessException.NotFound("数据源不存在");
        }

        // 2. 检查是否被报表引用（TODO: 等报表实体创建后实现）
        // var hasReports = await _db.Queryable<RptReport>()
        //     .Where(r => r.DatasourceId == id)
        //     .AnyAsync();
        //
        // if (hasReports)
        // {
        //     throw BusinessException.BadRequest("该数据源已被报表引用，无法删除");
        // }

        // 3. 删除数据源
        var result = await _db.Deleteable<RptDatasource>()
            .Where(d => d.Id == id)
            .ExecuteCommandAsync();

        if (result > 0)
        {
            _logger.LogInformation("删除数据源成功: {Name}", datasource.Name);
        }

        return result > 0;
    }

    /// <summary>
    /// 测试数据源连接
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <returns>连接测试结果，包含是否成功、错误信息等</returns>
    /// <remarks>
    /// 1. 根据数据源类型使用对应的连接器进行连接测试
    /// 2. 更新数据源的连接状态和最后测试时间
    /// 3. 记录测试结果和错误信息
    /// </remarks>
    public async Task<RptConnectionTestResultDto> TestConnectionAsync(Guid id)
    {
        // 1. 查询数据源
        var datasource = await _db.Queryable<RptDatasource>()
            .Where(d => d.Id == id)
            .FirstAsync();

        if (datasource == null)
        {
            throw BusinessException.NotFound("数据源不存在");
        }

        // 2. 解密密码
        var password = DecryptPassword(datasource.Password);

        // 3. 构建连接字符串
        var connectionString = BuildConnectionString(datasource, password);

        // 4. 测试连接
        var result = new RptConnectionTestResultDto();
        try
        {
            // 根据数据源类型创建连接
            using var connection = CreateDbConnection(datasource.Type, connectionString);
            await connection.OpenAsync();

            // 测试成功
            result.Success = true;
            result.Message = "连接成功";

            // 更新数据源状态
            datasource.Status = ReportConstants.DatasourceStatus.Connected;
            datasource.LastTestTime = DateTime.UtcNow;
            datasource.ErrorMessage = null;
        }
        catch (Exception ex)
        {
            // 测试失败
            result.Success = false;
            result.Message = "连接失败";
            result.ErrorMessage = ex.Message;

            // 更新数据源状态
            datasource.Status = ReportConstants.DatasourceStatus.Error;
            datasource.LastTestTime = DateTime.UtcNow;
            datasource.ErrorMessage = ex.Message;

            _logger.LogError(ex, "测试数据源连接失败: {Name}", datasource.Name);
        }

        // 5. 更新数据库
        await _db.Updateable(datasource)
            .UpdateColumns(d => new { d.Status, d.LastTestTime, d.ErrorMessage })
            .ExecuteCommandAsync();

        return result;
    }

    /// <summary>
    /// 获取所有数据源列表（不分页）
    /// </summary>
    /// <returns>所有数据源列表</returns>
    /// <remarks>
    /// 用于下拉选择框，只返回 ID、Name、Type 等基本字段
    /// </remarks>
    public async Task<List<RptDatasourceDto>> GetAllAsync()
    {
        // 1. 查询所有数据源
        var datasources = await _db.Queryable<RptDatasource>()
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

        // 2. 转换为 DTO 并脱敏密码
        var datasourceDtos = datasources.Adapt<List<RptDatasourceDto>>();
        for (int i = 0; i < datasourceDtos.Count; i++)
        {
            datasourceDtos[i].Password = "******";
            datasourceDtos[i].CreateTime = datasources[i].CreatedAt;
            datasourceDtos[i].UpdateTime = datasources[i].UpdatedAt ?? DateTime.MinValue;
        }

        return datasourceDtos;
    }

    #region 辅助方法

    /// <summary>
    /// 加密密码（Base64编码）
    /// </summary>
    /// <param name="password">原始密码</param>
    /// <returns>加密后的密码</returns>
    /// <remarks>
    /// 使用 Base64 编码对密码进行加密
    /// 注意：Base64 不是安全的加密方式，仅用于演示，实际生产环境应使用 AES 等加密算法
    /// </remarks>
    private string EncryptPassword(string password)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// 解密密码（Base64解码）
    /// </summary>
    /// <param name="encryptedPassword">加密后的密码</param>
    /// <returns>原始密码</returns>
    /// <remarks>
    /// 使用 Base64 解码对密码进行解密
    /// </remarks>
    private string DecryptPassword(string encryptedPassword)
    {
        var bytes = Convert.FromBase64String(encryptedPassword);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// 构建数据库连接字符串
    /// </summary>
    /// <param name="datasource">数据源信息</param>
    /// <param name="password">解密后的密码</param>
    /// <returns>数据库连接字符串</returns>
    /// <remarks>
    /// 根据数据源类型构建对应的连接字符串
    /// </remarks>
    private string BuildConnectionString(RptDatasource datasource, string password)
    {
        return datasource.Type switch
        {
            ReportConstants.DatasourceType.MySql =>
                $"Server={datasource.Host};Port={datasource.Port};Database={datasource.Database};User={datasource.Username};Password={password};Charset=utf8mb4;",
            ReportConstants.DatasourceType.PostgreSql =>
                $"Host={datasource.Host};Port={datasource.Port};Database={datasource.Database};Username={datasource.Username};Password={password};",
            ReportConstants.DatasourceType.SqlServer =>
                $"Server={datasource.Host},{datasource.Port};Database={datasource.Database};User Id={datasource.Username};Password={password};",
            ReportConstants.DatasourceType.Oracle =>
                $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={datasource.Host})(PORT={datasource.Port}))(CONNECT_DATA=(SERVICE_NAME={datasource.Database})));User Id={datasource.Username};Password={password};",
            _ => throw new NotSupportedException($"不支持的数据源类型: {datasource.Type}")
        };
    }

    /// <summary>
    /// 创建数据库连接对象
    /// </summary>
    /// <param name="type">数据源类型</param>
    /// <param name="connectionString">连接字符串</param>
    /// <returns>数据库连接对象</returns>
    /// <remarks>
    /// 根据数据源类型创建对应的数据库连接对象
    /// 注意：需要安装对应的数据库驱动包
    /// - MySQL: MySql.Data 或 MySqlConnector
    /// - PostgreSQL: Npgsql
    /// - SQL Server: Microsoft.Data.SqlClient
    /// - Oracle: Oracle.ManagedDataAccess
    /// </remarks>
    private System.Data.Common.DbConnection CreateDbConnection(string type, string connectionString)
    {
        // TODO: 需要安装对应的数据库驱动包
        // 实现方式：
        // 1. 安装 NuGet 包：
        //    - MySQL: dotnet add package MySqlConnector
        //    - PostgreSQL: dotnet add package Npgsql
        //    - SQL Server: dotnet add package Microsoft.Data.SqlClient
        //    - Oracle: dotnet add package Oracle.ManagedDataAccess.Core
        // 2. 取消下面的注释

        /*
        return type switch
        {
            ReportConstants.DatasourceType.MySql =>
                new MySqlConnector.MySqlConnection(connectionString),
            ReportConstants.DatasourceType.PostgreSql =>
                new Npgsql.NpgsqlConnection(connectionString),
            ReportConstants.DatasourceType.SqlServer =>
                new Microsoft.Data.SqlClient.SqlConnection(connectionString),
            ReportConstants.DatasourceType.Oracle =>
                new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString),
            _ => throw new NotSupportedException($"不支持的数据源类型: {type}")
        };
        */

        throw new NotImplementedException($"请先安装 {type} 数据库驱动包，然后实现 CreateDbConnection 方法");
    }

    #endregion
}