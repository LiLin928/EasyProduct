using System.Data;
using System.Text;
using System.Text.Json;
using EasyProduct.Common.Base;
using EasyProduct.Models.Constants;
using EasyProduct.Models.Dto.Report.Definition;
using EasyProduct.Models.Entitys.Report;
using Mapster;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using SqlSugar;

namespace EasyProduct.Business.Report;

/// <summary>
/// 报表定义服务实现
/// </summary>
public class RptDefinitionService : IRptDefinitionService
{
    private readonly ISqlSugarClient _db;
    private readonly ILogger<RptDefinitionService> _logger;

    // SQL 注入黑名单关键词
    private static readonly string[] SqlBlacklist = new[]
    {
        "DROP", "DELETE", "TRUNCATE", "ALTER", "CREATE", "EXEC", "INSERT", "UPDATE"
    };

    // SQL 注入危险符号
    private static readonly string[] DangerousSymbols = new[]
    {
        "--", "/*", "*/", ";"
    };

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端</param>
    /// <param name="logger">日志记录器</param>
    public RptDefinitionService(ISqlSugarClient db, ILogger<RptDefinitionService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// 获取报表定义分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>报表定义分页列表</returns>
    /// <remarks>
    /// 1. 支持 LeftJoin 数据源表获取数据源名称
    /// 2. 支持按 Name、Code、Category、DatasourceId、Status 筛选
    /// 3. 解析 JSON 列配置
    /// </remarks>
    public async Task<PageResponse<RptDefinitionDto>> GetListAsync(RptDefinitionQuery query)
    {
        var queryable = _db.Queryable<RptDefinition>()
            .LeftJoin<RptDatasource>((d, ds) => d.DatasourceId == ds.Id)
            .WhereIF(!string.IsNullOrWhiteSpace(query.Name), (d, ds) => d.Name.Contains(query.Name!))
            .WhereIF(!string.IsNullOrWhiteSpace(query.Code), (d, ds) => d.Code == query.Code)
            .WhereIF(!string.IsNullOrWhiteSpace(query.Category), (d, ds) => d.Category == query.Category)
            .WhereIF(!string.IsNullOrWhiteSpace(query.DatasourceId), (d, ds) => d.DatasourceId == Guid.Parse(query.DatasourceId!))
            .WhereIF(query.Status.HasValue, (d, ds) => d.Status == query.Status)
            .Select((d, ds) => new RptDefinitionDto
            {
                Id = d.Id.ToString(),
                Name = d.Name,
                Code = d.Code,
                Category = d.Category,
                DatasourceId = d.DatasourceId.ToString(),
                DatasourceName = ds.Name,
                SqlTemplate = d.SqlTemplate,
                ChartType = d.ChartType,
                Status = d.Status,
                Remark = d.Remark,
                CreateTime = d.CreatedAt,
                UpdateTime = d.UpdatedAt ?? DateTime.UtcNow
            });

        // 分页查询
        var total = 0;
        var list = await queryable
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 解析 JSON 列配置
        foreach (var item in list)
        {
            var entity = await _db.Queryable<RptDefinition>()
                .Where(d => d.Id == Guid.Parse(item.Id))
                .FirstAsync();

            if (entity != null && !string.IsNullOrWhiteSpace(entity.Columns))
            {
                try
                {
                    item.Columns = JsonSerializer.Deserialize<List<RptReportColumnDto>>(entity.Columns) ?? new List<RptReportColumnDto>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "解析报表 {ReportId} 的列配置失败", item.Id);
                    item.Columns = new List<RptReportColumnDto>();
                }
            }
        }

        return new PageResponse<RptDefinitionDto>
        {
            List = list,
            Total = total
        };
    }

    /// <summary>
    /// 获取报表定义详情
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <returns>报表定义详情</returns>
    /// <remarks>
    /// 包含数据源名称，解析 JSON 列配置
    /// </remarks>
    public async Task<RptDefinitionDto> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<RptDefinition>()
            .LeftJoin<RptDatasource>((d, ds) => d.DatasourceId == ds.Id)
            .Where((d, ds) => d.Id == id)
            .Select((d, ds) => new RptDefinitionDto
            {
                Id = d.Id.ToString(),
                Name = d.Name,
                Code = d.Code,
                Category = d.Category,
                DatasourceId = d.DatasourceId.ToString(),
                DatasourceName = ds.Name,
                SqlTemplate = d.SqlTemplate,
                ChartType = d.ChartType,
                Status = d.Status,
                Remark = d.Remark,
                CreateTime = d.CreatedAt,
                UpdateTime = d.UpdatedAt ?? DateTime.UtcNow
            })
            .FirstAsync();

        if (entity == null)
        {
            throw new Exception($"报表定义 {id} 不存在");
        }

        // 解析 JSON 列配置
        var definition = await _db.Queryable<RptDefinition>()
            .Where(d => d.Id == id)
            .FirstAsync();

        if (definition != null && !string.IsNullOrWhiteSpace(definition.Columns))
        {
            try
            {
                entity.Columns = JsonSerializer.Deserialize<List<RptReportColumnDto>>(definition.Columns) ?? new List<RptReportColumnDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解析报表 {ReportId} 的列配置失败", id);
                entity.Columns = new List<RptReportColumnDto>();
            }
        }

        return entity;
    }

    /// <summary>
    /// 创建报表定义
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新报表定义ID</returns>
    /// <remarks>
    /// 1. 检查编码唯一性
    /// 2. 检查数据源是否存在
    /// 3. JSON 序列化列配置
    /// </remarks>
    public async Task<Guid> CreateAsync(RptDefinitionCreateDto dto)
    {
        // 检查编码唯一性
        var exists = await _db.Queryable<RptDefinition>()
            .Where(d => d.Code == dto.Code)
            .AnyAsync();

        if (exists)
        {
            throw new Exception($"报表编码 {dto.Code} 已存在");
        }

        // 检查数据源是否存在
        var datasourceExists = await _db.Queryable<RptDatasource>()
            .Where(ds => ds.Id == Guid.Parse(dto.DatasourceId))
            .AnyAsync();

        if (!datasourceExists)
        {
            throw new Exception($"数据源 {dto.DatasourceId} 不存在");
        }

        // SQL 注入检查
        ValidateSqlSecurity(dto.SqlTemplate);

        // 序列化列配置
        var columnsJson = JsonSerializer.Serialize(dto.Columns);

        var entity = new RptDefinition
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            Category = dto.Category,
            DatasourceId = Guid.Parse(dto.DatasourceId),
            SqlTemplate = dto.SqlTemplate,
            ChartType = dto.ChartType,
            Columns = columnsJson,
            Status = ReportConstants.ReportStatus.Draft,
            Remark = dto.Remark,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();

        _logger.LogInformation("创建报表定义 {ReportId}, 名称: {Name}, 编码: {Code}", entity.Id, entity.Name, entity.Code);

        return entity.Id;
    }

    /// <summary>
    /// 更新报表定义
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 1. 检查编码唯一性（排除自己）
    /// 2. JSON 序列化列配置
    /// </remarks>
    public async Task<bool> UpdateAsync(Guid id, RptDefinitionUpdateDto dto)
    {
        var entity = await _db.Queryable<RptDefinition>()
            .Where(d => d.Id == id)
            .FirstAsync();

        if (entity == null)
        {
            throw new Exception($"报表定义 {id} 不存在");
        }

        // 检查编码唯一性（排除自己）
        var exists = await _db.Queryable<RptDefinition>()
            .Where(d => d.Code == dto.Code && d.Id != id)
            .AnyAsync();

        if (exists)
        {
            throw new Exception($"报表编码 {dto.Code} 已存在");
        }

        // SQL 注入检查
        ValidateSqlSecurity(dto.SqlTemplate);

        // 序列化列配置
        var columnsJson = JsonSerializer.Serialize(dto.Columns);

        entity.Name = dto.Name;
        entity.Code = dto.Code;
        entity.Category = dto.Category;
        entity.DatasourceId = Guid.Parse(dto.DatasourceId);
        entity.SqlTemplate = dto.SqlTemplate;
        entity.ChartType = dto.ChartType;
        entity.Columns = columnsJson;
        entity.Status = dto.Status;
        entity.Remark = dto.Remark;
        entity.UpdatedAt = DateTime.Now;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("更新报表定义 {ReportId}, 名称: {Name}", id, entity.Name);

        return true;
    }

    /// <summary>
    /// 删除报表定义
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _db.Queryable<RptDefinition>()
            .Where(d => d.Id == id)
            .FirstAsync();

        if (entity == null)
        {
            throw new Exception($"报表定义 {id} 不存在");
        }

        // 检查是否已发布
        if (entity.Status == ReportConstants.ReportStatus.Published)
        {
            throw new Exception("已发布的报表不能删除，请先归档");
        }

        await _db.Deleteable(entity).ExecuteCommandAsync();

        _logger.LogInformation("删除报表定义 {ReportId}, 名称: {Name}", id, entity.Name);

        return true;
    }

    /// <summary>
    /// 执行报表查询
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页条数</param>
    /// <returns>查询结果</returns>
    /// <remarks>
    /// 1. SQL 注入防护（黑名单检查）
    /// 2. 只允许 SELECT 语句
    /// 3. 使用 MySqlConnector 执行查询
    /// 4. 支持分页
    /// </remarks>
    public async Task<RptExecuteResultDto> ExecuteAsync(Guid id, int pageIndex = 1, int pageSize = 100)
    {
        var definition = await _db.Queryable<RptDefinition>()
            .Where(d => d.Id == id)
            .FirstAsync();

        if (definition == null)
        {
            throw new Exception($"报表定义 {id} 不存在");
        }

        // 检查报表状态
        if (definition.Status != ReportConstants.ReportStatus.Published)
        {
            throw new Exception("只有已发布的报表才能执行");
        }

        // SQL 注入检查
        ValidateSqlSecurity(definition.SqlTemplate);

        // 检查是否为 SELECT 语句
        if (!IsSelectStatement(definition.SqlTemplate))
        {
            throw new Exception("报表查询只允许使用 SELECT 语句");
        }

        // 获取数据源
        var datasource = await _db.Queryable<RptDatasource>()
            .Where(ds => ds.Id == definition.DatasourceId)
            .FirstAsync();

        if (datasource == null)
        {
            throw new Exception($"数据源 {definition.DatasourceId} 不存在");
        }

        // 构建连接字符串
        var connectionString = BuildConnectionString(datasource);

        // 执行查询
        var rows = new List<Dictionary<string, object>>();
        var total = 0;

        using (var connection = new MySqlConnector.MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            // 构建分页查询
            var countSql = $"SELECT COUNT(*) FROM ({definition.SqlTemplate}) AS count_query";
            var dataSql = $"{definition.SqlTemplate} LIMIT {(pageIndex - 1) * pageSize}, {pageSize}";

            // 获取总条数
            using (var countCommand = new MySqlConnector.MySqlCommand(countSql, connection))
            {
                var result = await countCommand.ExecuteScalarAsync();
                total = Convert.ToInt32(result);
            }

            // 获取数据
            using (var dataCommand = new MySqlConnector.MySqlCommand(dataSql, connection))
            {
                using (var reader = await dataCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var fieldName = reader.GetName(i);
                            var fieldValue = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            row[fieldName] = fieldValue;
                        }
                        rows.Add(row);
                    }
                }
            }
        }

        // 解析列配置
        List<RptReportColumnDto> columns = new List<RptReportColumnDto>();
        if (!string.IsNullOrWhiteSpace(definition.Columns))
        {
            try
            {
                columns = JsonSerializer.Deserialize<List<RptReportColumnDto>>(definition.Columns) ?? new List<RptReportColumnDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解析报表 {ReportId} 的列配置失败", id);
            }
        }

        _logger.LogInformation("执行报表查询 {ReportId}, 返回 {Count} 条记录", id, rows.Count);

        return new RptExecuteResultDto
        {
            Columns = columns,
            Rows = rows,
            Total = total
        };
    }

    /// <summary>
    /// 导出报表数据到 Excel
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <returns>Excel 文件字节数组</returns>
    /// <remarks>
    /// 使用 MiniExcel 导出
    /// </remarks>
    public async Task<byte[]> ExportToExcelAsync(Guid id)
    {
        // 执行报表查询（不分页，获取所有数据）
        var result = await ExecuteAsync(id, 1, 10000);

        // 使用 MiniExcel 导出
        using (var stream = new MemoryStream())
        {
            await stream.SaveAsAsync(result.Rows);
            return stream.ToArray();
        }
    }

    /// <summary>
    /// 发布报表
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> PublishAsync(Guid id)
    {
        var entity = await _db.Queryable<RptDefinition>()
            .Where(d => d.Id == id)
            .FirstAsync();

        if (entity == null)
        {
            throw new Exception($"报表定义 {id} 不存在");
        }

        if (entity.Status == ReportConstants.ReportStatus.Published)
        {
            throw new Exception("报表已发布，无需重复操作");
        }

        if (entity.Status == ReportConstants.ReportStatus.Archived)
        {
            throw new Exception("已归档的报表不能发布");
        }

        // SQL 注入检查
        ValidateSqlSecurity(entity.SqlTemplate);

        entity.Status = ReportConstants.ReportStatus.Published;
        entity.UpdatedAt = DateTime.Now;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("发布报表定义 {ReportId}, 名称: {Name}", id, entity.Name);

        return true;
    }

    /// <summary>
    /// 归档报表
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> ArchiveAsync(Guid id)
    {
        var entity = await _db.Queryable<RptDefinition>()
            .Where(d => d.Id == id)
            .FirstAsync();

        if (entity == null)
        {
            throw new Exception($"报表定义 {id} 不存在");
        }

        if (entity.Status == ReportConstants.ReportStatus.Archived)
        {
            throw new Exception("报表已归档，无需重复操作");
        }

        entity.Status = ReportConstants.ReportStatus.Archived;
        entity.UpdatedAt = DateTime.Now;

        await _db.Updateable(entity).ExecuteCommandAsync();

        _logger.LogInformation("归档报表定义 {ReportId}, 名称: {Name}", id, entity.Name);

        return true;
    }

    /// <summary>
    /// 验证 SQL 安全性
    /// </summary>
    /// <param name="sql">要验证的 SQL 语句</param>
    /// <remarks>
    /// 1. 检查黑名单关键词：DROP、DELETE、TRUNCATE、ALTER、CREATE、EXEC、INSERT、UPDATE
    /// 2. 检查危险符号：--、/*、*/、;
    /// </remarks>
    private void ValidateSqlSecurity(string sql)
    {
        var upperSql = sql.ToUpper().Trim();

        // 检查黑名单关键词
        foreach (var keyword in SqlBlacklist)
        {
            if (upperSql.Contains(keyword))
            {
                _logger.LogWarning("SQL 包含危险关键词: {Keyword}", keyword);
                throw new Exception($"SQL 包含危险关键词: {keyword}");
            }
        }

        // 检查危险符号
        foreach (var symbol in DangerousSymbols)
        {
            if (sql.Contains(symbol))
            {
                _logger.LogWarning("SQL 包含危险符号: {Symbol}", symbol);
                throw new Exception($"SQL 包含危险符号: {symbol}");
            }
        }
    }

    /// <summary>
    /// 检查是否为 SELECT 语句
    /// </summary>
    /// <param name="sql">要检查的 SQL 语句</param>
    /// <returns>是否为 SELECT 语句</returns>
    private bool IsSelectStatement(string sql)
    {
        var trimmedSql = sql.Trim().ToUpper();
        return trimmedSql.StartsWith("SELECT") || trimmedSql.StartsWith("WITH");
    }

    /// <summary>
    /// 构建数据库连接字符串
    /// </summary>
    /// <param name="datasource">数据源配置</param>
    /// <returns>连接字符串</returns>
    private string BuildConnectionString(RptDatasource datasource)
    {
        var builder = new MySqlConnector.MySqlConnectionStringBuilder
        {
            Server = datasource.Host,
            Port = (uint)datasource.Port,
            Database = datasource.Database,
            UserID = datasource.Username,
            Password = datasource.Password,
            CharacterSet = "utf8mb4",
            SslMode = MySqlConnector.MySqlSslMode.None
        };

        return builder.ConnectionString;
    }
}