# Report 与 Ops 模块后端开发说明

> **适用范围**：EasyProduct.WebApi 的 Report（报表引擎）和 Ops（运维管理）模块
> **上游依据**：`docs/api/other-modules.md`、`docs/backend-guidelines.md`
> **开发阶段**：P4（报表引擎）、P5（运维管理）

**关键设计规范（2026-09-07 更新）：**
- 所有实体类继承 `BaseEntity`
- 状态字段使用 `int` 类型，配合状态常量类使用
- `IsDeleted` 字段使用 `int` 类型（0-未删除，1-已删除）

---

## 一、Report 模块 - 报表引擎

报表引擎模块，支持自定义报表、数据源配置、查询执行与导出功能。

### 1.1 功能概述

- **数据源管理**：支持 MySQL、PostgreSQL、SQL Server、Oracle 等多种数据源
- **报表定义**：可视化配置报表字段、筛选条件、列宽等
- **查询执行**：安全执行 SQL 查询，支持参数化
- **数据导出**：支持 Excel 导出
- **权限控制**：数据权限过滤，防止越权访问

### 1.2 实体类设计

#### RptDatasource - 数据源

```csharp
namespace EasyProduct.Models.Entitys.Report;

/// <summary>
/// 报表数据源
/// </summary>
[SugarTable("rpt_datasource", "报表数据源表")]
public class RptDatasource : BaseEntity
{
    /// <summary>
    /// 数据源名称
    /// </summary>
    [SugarColumn(Length = 100)]
    [Required(ErrorMessage = "数据源名称不能为空")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 数据源类型：mysql/postgresql/sqlserver/oracle
    /// </summary>
    [SugarColumn(Length = 20)]
    [Required(ErrorMessage = "数据源类型不能为空")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 主机地址
    /// </summary>
    [SugarColumn(Length = 100)]
    [Required(ErrorMessage = "主机地址不能为空")]
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// 端口号
    /// </summary>
    [Required(ErrorMessage = "端口号不能为空")]
    public int Port { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    [SugarColumn(Length = 100)]
    [Required(ErrorMessage = "数据库名称不能为空")]
    public string Database { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 50)]
    [Required(ErrorMessage = "用户名不能为空")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码（加密存储）
    /// </summary>
    [SugarColumn(Length = 200)]
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 连接状态：1-已连接，2-连接错误
    /// </summary>
    public int Status { get; set; } = DatasourceStatus.Error;

    /// <summary>
    /// 最后测试时间
    /// </summary>
    public DateTime? LastTestTime { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 数据源状态常量
/// </summary>
public static class DatasourceStatus
{
    /// <summary>
    /// 已连接
    /// </summary>
    public const int Connected = 1;

    /// <summary>
    /// 连接错误
    /// </summary>
    public const int Error = 2;
}
```


#### RptDefinition - 报表定义

```csharp
namespace EasyProduct.Models.Entitys.Report;

/// <summary>
/// 报表定义
/// </summary>
[SugarTable("rpt_definition", "报表定义表")]
public class RptDefinition : BaseEntity
{
    /// <summary>
    /// 报表名称
    /// </summary>
    [SugarColumn(Length = 100)]
    [Required(ErrorMessage = "报表名称不能为空")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 报表分类
    /// </summary>
    [SugarColumn(Length = 50)]
    [Required(ErrorMessage = "报表分类不能为空")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 数据源ID
    /// </summary>
    [Required(ErrorMessage = "数据源不能为空")]
    public Guid DatasourceId { get; set; }

    /// <summary>
    /// 查询SQL（支持参数化）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    [Required(ErrorMessage = "查询SQL不能为空")]
    public string Sql { get; set; } = string.Empty;

    /// <summary>
    /// 列定义（JSON）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Columns { get; set; } = "[]";

    /// <summary>
    /// 筛选条件（JSON）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Filters { get; set; } = "[]";

    /// <summary>
    /// 状态：1-草稿，2-已发布
    /// </summary>
    public int Status { get; set; } = ReportStatus.Draft;

    /// <summary>
    /// 备注说明
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }
}

/// <summary>
/// 报表状态常量
/// </summary>
public static class ReportStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    public const int Draft = 1;

    /// <summary>
    /// 已发布
    /// </summary>
    public const int Published = 2;
}
```

#### RptColumnTemplate - 列模板

```csharp
namespace EasyProduct.Models.Entitys.Report;

/// <summary>
/// 报表列模板
/// </summary>
[SugarTable("rpt_column_template", "报表列模板表")]
public class RptColumnTemplate : BaseEntity
{
    /// <summary>
    /// 模板名称
    /// </summary>
    [SugarColumn(Length = 100)]
    [Required(ErrorMessage = "模板名称不能为空")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 模板分类
    /// </summary>
    [SugarColumn(Length = 50)]
    [Required(ErrorMessage = "模板分类不能为空")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 列定义（JSON）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Columns { get; set; } = "[]";

    /// <summary>
    /// 备注说明
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }
}
```

### 1.3 Service 层实现

#### IRptDatasourceService - 数据源服务接口

```csharp
namespace EasyProduct.Business.Report;

/// <summary>
/// 报表数据源服务接口
/// </summary>
public interface IRptDatasourceService
{
    /// <summary>
    /// 获取数据源列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>数据源分页列表</returns>
    Task<PageResult<RptDatasourceDto>> GetListAsync(RptDatasourceQuery query);

    /// <summary>
    /// 获取数据源详情
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <returns>数据源详情</returns>
    Task<RptDatasourceDto> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建数据源
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新数据源ID</returns>
    Task<Guid> CreateAsync(RptDatasourceCreateDto dto);

    /// <summary>
    /// 更新数据源
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(Guid id, RptDatasourceUpdateDto dto);

    /// <summary>
    /// 删除数据源
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 测试数据源连接
    /// </summary>
    /// <param name="id">数据源ID</param>
    /// <returns>测试结果</returns>
    Task<RptConnectionTestResultDto> TestConnectionAsync(Guid id);
}
```

### 1.4 业务逻辑要点

#### 1. SQL 注入防护（强制）

**实现策略：**

1. **参数化查询**：所有参数必须使用 `SugarParameter`，禁止字符串拼接
2. **参数名白名单**：只允许字母、数字、下划线，长度限制 50
3. **SQL 关键字黑名单**：检测 `DROP`、`DELETE`、`TRUNCATE`、`ALTER`、`CREATE`、`EXEC` 等危险关键字
4. **注释符号检测**：禁止 `--`、`/*`、`*/`、`;`
5. **只允许 SELECT**：报表查询仅允许 `SELECT` 语句

#### 2. 数据权限控制

**权限级别：**

1. **报表级权限**：报表定义关联角色/用户，控制谁能访问
2. **数据级权限**：根据当前用户的数据权限范围过滤数据
3. **字段级权限**：敏感字段脱敏或不展示

#### 3. 大数据查询优化

**优化策略：**

1. **分页强制**：大数据量必须分页，pageSize 上限 1000
2. **超时控制**：查询超时 30 秒，超时返回友好提示
3. **异步执行**：超长查询转为异步任务，完成后通知
4. **缓存策略**：静态报表结果缓存 5 分钟（可配置）

#### 4. 密码加密存储

数据源密码必须加密存储，返回时脱敏显示 `******`

---

## 二、Ops 模块 - 运维管理

运维管理模块，包含操作日志、登录日志、定时任务管理等。

### 2.1 功能概述

- **操作日志**：记录用户操作行为，支持查询和导出
- **登录日志**：记录登录成功/失败，支持地理定位
- **定时任务**：Quartz 调度管理，支持启动/暂停/立即执行
- **任务日志**：记录任务执行历史

### 2.2 实体类设计

#### OpsOperateLog - 操作日志

```csharp
namespace EasyProduct.Models.Entitys.Ops;

/// <summary>
/// 操作日志
/// </summary>
[SugarTable("ops_operate_log", "操作日志表")]
public class OpsOperateLog : BaseEntity
{
    /// <summary>
    /// 模块名称
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// 操作类型
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// 业务ID
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? BusinessId { get; set; }

    /// <summary>
    /// 业务名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? BusinessName { get; set; }

    /// <summary>
    /// 操作人ID
    /// </summary>
    [SugarColumn(Length = 50)]
    public string OperatorId { get; set; } = string.Empty;

    /// <summary>
    /// 操作人姓名
    /// </summary>
    [SugarColumn(Length = 50)]
    public string OperatorName { get; set; } = string.Empty;

    /// <summary>
    /// 请求方法
    /// </summary>
    [SugarColumn(Length = 10)]
    public string RequestMethod { get; set; } = string.Empty;

    /// <summary>
    /// 请求URL
    /// </summary>
    [SugarColumn(Length = 500)]
    public string RequestUrl { get; set; } = string.Empty;

    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? RequestParams { get; set; }

    /// <summary>
    /// 响应结果
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? ResponseResult { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Ip { get; set; } = string.Empty;

    /// <summary>
    /// 执行状态：1-成功，2-失败
    /// </summary>
    public int Status { get; set; } = OperateStatus.Success;

    /// <summary>
    /// 执行时长（毫秒）
    /// </summary>
    public long Duration { get; set; }

    /// <summary>
    /// 追踪ID
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? TraceId { get; set; }
}

/// <summary>
/// 操作状态常量
/// </summary>
public static class OperateStatus
{
    /// <summary>
    /// 成功
    /// </summary>
    public const int Success = 1;

    /// <summary>
    /// 失败
    /// </summary>
    public const int Failed = 2;
}
```

#### OpsLoginLog - 登录日志

```csharp
namespace EasyProduct.Models.Entitys.Ops;

/// <summary>
/// 登录日志
/// </summary>
[SugarTable("ops_login_log", "登录日志表")]
public class OpsLoginLog : BaseEntity
{
    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 50)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 登录类型：admin/member
    /// </summary>
    [SugarColumn(Length = 20)]
    public string LoginType { get; set; } = "admin";

    /// <summary>
    /// 登录方式：password/sms/wechat
    /// </summary>
    [SugarColumn(Length = 20)]
    public string LoginMethod { get; set; } = "password";

    /// <summary>
    /// IP地址
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Ip { get; set; } = string.Empty;

    /// <summary>
    /// 地理位置
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Location { get; set; }

    /// <summary>
    /// 浏览器
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Browser { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Os { get; set; }

    /// <summary>
    /// 登录状态：1-成功，2-失败
    /// </summary>
    public int Status { get; set; } = LoginStatus.Success;

    /// <summary>
    /// 提示信息
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Message { get; set; }
}

/// <summary>
/// 登录状态常量
/// </summary>
public static class LoginStatus
{
    /// <summary>
    /// 成功
    /// </summary>
    public const int Success = 1;

    /// <summary>
    /// 失败
    /// </summary>
    public const int Failed = 2;
}
```

#### OpsTask - 定时任务

```csharp
namespace EasyProduct.Models.Entitys.Ops;

/// <summary>
/// 定时任务
/// </summary>
[SugarTable("ops_task", "定时任务表")]
public class OpsTask : BaseEntity
{
    /// <summary>
    /// 任务名称
    /// </summary>
    [SugarColumn(Length = 100)]
    [Required(ErrorMessage = "任务名称不能为空")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 任务分组
    /// </summary>
    [SugarColumn(Length = 50)]
    [Required(ErrorMessage = "任务分组不能为空")]
    public string Group { get; set; } = "default";

    /// <summary>
    /// Cron表达式
    /// </summary>
    [SugarColumn(Length = 100)]
    [Required(ErrorMessage = "Cron表达式不能为空")]
    public string Cron { get; set; } = string.Empty;

    /// <summary>
    /// 执行类名
    /// </summary>
    [SugarColumn(Length = 200)]
    [Required(ErrorMessage = "执行类名不能为空")]
    public string ClassName { get; set; } = string.Empty;

    /// <summary>
    /// 执行方法名
    /// </summary>
    [SugarColumn(Length = 50)]
    [Required(ErrorMessage = "执行方法名不能为空")]
    public string MethodName { get; set; } = "Execute";

    /// <summary>
    /// 执行参数（JSON）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Params { get; set; }

    /// <summary>
    /// 任务状态：1-运行中，2-已暂停
    /// </summary>
    public int Status { get; set; } = TaskStatus.Paused;

    /// <summary>
    /// 上次执行时间
    /// </summary>
    public DateTime? LastExecuteTime { get; set; }

    /// <summary>
    /// 下次执行时间
    /// </summary>
    public DateTime? NextExecuteTime { get; set; }

    /// <summary>
    /// 备注说明
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }
}

/// <summary>
/// 任务状态常量
/// </summary>
public static class TaskStatus
{
    /// <summary>
    /// 运行中
    /// </summary>
    public const int Running = 1;

    /// <summary>
    /// 已暂停
    /// </summary>
    public const int Paused = 2;
}
```

#### OpsTaskLog - 任务日志

```csharp
namespace EasyProduct.Models.Entitys.Ops;

/// <summary>
/// 任务执行日志
/// </summary>
[SugarTable("ops_task_log", "任务执行日志表")]
public class OpsTaskLog : BaseEntity
{
    /// <summary>
    /// 任务ID
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    [SugarColumn(Length = 100)]
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 执行时间
    /// </summary>
    public DateTime ExecuteTime { get; set; }

    /// <summary>
    /// 执行时长（毫秒）
    /// </summary>
    public long Duration { get; set; }

    /// <summary>
    /// 执行状态：1-成功，2-失败
    /// </summary>
    public int Status { get; set; } = TaskExecuteStatus.Success;

    /// <summary>
    /// 执行结果
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Result { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? ErrorMsg { get; set; }
}

/// <summary>
/// 任务执行状态常量
/// </summary>
public static class TaskExecuteStatus
{
    /// <summary>
    /// 成功
    /// </summary>
    public const int Success = 1;

    /// <summary>
    /// 失败
    /// </summary>
    public const int Failed = 2;
}
```

### 2.3 Service 层实现

#### IOpsOperateLogService - 操作日志服务接口

```csharp
namespace EasyProduct.Business.Ops;

public interface IOpsOperateLogService
{
    Task<PageResult<OpsOperateLogDto>> GetListAsync(OpsOperateLogQuery query);
    Task<OpsOperateLogDto> GetByIdAsync(Guid id);
    Task<bool> LogAsync(OpsOperateLogCreateDto dto);
    Task<byte[]> ExportAsync(OpsOperateLogQuery query);
    Task<int> CleanAsync(int days);
}
```

### 2.4 业务逻辑要点

#### 1. 操作日志自动记录

使用 ActionFilter 自动拦截并记录操作日志，通过 `[OperateLog]` 特性标注。

#### 2. 登录日志记录

在登录成功/失败时记录登录日志，包含用户名、IP、浏览器、操作系统等信息。

#### 3. 日志存储策略

**策略要点：**

1. **操作日志保留 180 天**：超过自动清理
2. **登录日志保留 365 天**：安全审计需要
3. **任务日志保留 30 天**：任务执行记录
4. **日志表分区**：按月分区，提升查询性能
5. **索引优化**：`CreateTime`、`OperatorId`、`Module` 等字段建立索引

#### 4. Quartz 任务调度集成

使用 Quartz.NET 实现定时任务调度，支持 Cron 表达式配置。

#### 5. 任务执行基类

提供统一的任务执行基类，自动记录任务执行日志。

---

## 三、数据库表设计（SQL）

### 3.1 Report 模块表

```sql
-- 报表数据源表
CREATE TABLE `rpt_datasource` (
  `id` char(36) NOT NULL COMMENT '主键ID',
  `name` varchar(100) NOT NULL COMMENT '数据源名称',
  `type` varchar(20) NOT NULL COMMENT '数据源类型',
  `host` varchar(100) NOT NULL COMMENT '主机地址',
  `port` int NOT NULL COMMENT '端口号',
  `database` varchar(100) NOT NULL COMMENT '数据库名称',
  `username` varchar(50) NOT NULL COMMENT '用户名',
  `password` varchar(200) NOT NULL COMMENT '密码',
  `status` int NOT NULL DEFAULT '2' COMMENT '连接状态：1-已连接，2-连接错误',
  `last_test_time` datetime DEFAULT NULL COMMENT '最后测试时间',
  `error_message` varchar(500) DEFAULT NULL COMMENT '错误信息',
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `update_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `create_by` varchar(50) DEFAULT NULL,
  `is_deleted` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `idx_name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='报表数据源表';

-- 报表定义表
CREATE TABLE `rpt_definition` (
  `id` char(36) NOT NULL COMMENT '主键ID',
  `name` varchar(100) NOT NULL COMMENT '报表名称',
  `category` varchar(50) NOT NULL COMMENT '报表分类',
  `datasource_id` char(36) NOT NULL COMMENT '数据源ID',
  `sql` text NOT NULL COMMENT '查询SQL',
  `columns` text COMMENT '列定义',
  `filters` text COMMENT '筛选条件',
  `status` int NOT NULL DEFAULT '1' COMMENT '状态：1-草稿，2-已发布',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注说明',
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `update_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `create_by` varchar(50) DEFAULT NULL,
  `is_deleted` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `idx_name` (`name`),
  KEY `idx_datasource` (`datasource_id`),
  CONSTRAINT `fk_definition_datasource` FOREIGN KEY (`datasource_id`) REFERENCES `rpt_datasource` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='报表定义表';

-- 报表列模板表
CREATE TABLE `rpt_column_template` (
  `id` char(36) NOT NULL,
  `name` varchar(100) NOT NULL,
  `category` varchar(50) NOT NULL,
  `columns` text,
  `remark` varchar(500) DEFAULT NULL COMMENT '备注说明',
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `update_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `create_by` varchar(50) DEFAULT NULL,
  `is_deleted` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `idx_name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='报表列模板表';
```

### 3.2 Ops 模块表

```sql
-- 操作日志表
CREATE TABLE `ops_operate_log` (
  `id` char(36) NOT NULL,
  `module` varchar(50) NOT NULL COMMENT '模块名称',
  `action` varchar(50) NOT NULL COMMENT '操作类型',
  `business_id` varchar(50) DEFAULT NULL COMMENT '业务ID',
  `business_name` varchar(200) DEFAULT NULL COMMENT '业务名称',
  `operator_id` varchar(50) NOT NULL COMMENT '操作人ID',
  `operator_name` varchar(50) NOT NULL COMMENT '操作人姓名',
  `request_method` varchar(10) NOT NULL COMMENT '请求方法',
  `request_url` varchar(500) NOT NULL COMMENT '请求URL',
  `request_params` text COMMENT '请求参数',
  `response_result` text COMMENT '响应结果',
  `ip` varchar(50) NOT NULL COMMENT 'IP地址',
  `status` int NOT NULL COMMENT '执行状态：1-成功，2-失败',
  `duration` bigint NOT NULL COMMENT '执行时长（毫秒）',
  `trace_id` varchar(50) DEFAULT NULL COMMENT '追踪ID',
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `update_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `is_deleted` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `idx_module` (`module`),
  KEY `idx_operator` (`operator_id`),
  KEY `idx_create_time` (`create_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='操作日志表';

-- 登录日志表
CREATE TABLE `ops_login_log` (
  `id` char(36) NOT NULL,
  `user_name` varchar(50) NOT NULL COMMENT '用户名',
  `login_type` varchar(20) NOT NULL COMMENT '登录类型',
  `login_method` varchar(20) NOT NULL COMMENT '登录方式',
  `ip` varchar(50) NOT NULL COMMENT 'IP地址',
  `location` varchar(200) DEFAULT NULL COMMENT '地理位置',
  `browser` varchar(100) DEFAULT NULL COMMENT '浏览器',
  `os` varchar(100) DEFAULT NULL COMMENT '操作系统',
  `status` int NOT NULL COMMENT '登录状态：1-成功，2-失败',
  `message` varchar(500) DEFAULT NULL COMMENT '提示信息',
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `update_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `is_deleted` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `idx_user_name` (`user_name`),
  KEY `idx_create_time` (`create_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='登录日志表';

-- 定时任务表
CREATE TABLE `ops_task` (
  `id` char(36) NOT NULL,
  `name` varchar(100) NOT NULL COMMENT '任务名称',
  `group` varchar(50) NOT NULL DEFAULT 'default' COMMENT '任务分组',
  `cron` varchar(100) NOT NULL COMMENT 'Cron表达式',
  `class_name` varchar(200) NOT NULL COMMENT '执行类名',
  `method_name` varchar(50) NOT NULL DEFAULT 'Execute' COMMENT '执行方法名',
  `params` text COMMENT '执行参数（JSON）',
  `status` int NOT NULL DEFAULT '2' COMMENT '任务状态：1-运行中，2-已暂停',
  `last_execute_time` datetime DEFAULT NULL COMMENT '上次执行时间',
  `next_execute_time` datetime DEFAULT NULL COMMENT '下次执行时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注说明',
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `update_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `is_deleted` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `idx_name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='定时任务表';

-- 任务执行日志表
CREATE TABLE `ops_task_log` (
  `id` char(36) NOT NULL,
  `task_id` char(36) NOT NULL COMMENT '任务ID',
  `task_name` varchar(100) NOT NULL COMMENT '任务名称',
  `execute_time` datetime NOT NULL COMMENT '执行时间',
  `duration` bigint NOT NULL COMMENT '执行时长（毫秒）',
  `status` int NOT NULL COMMENT '执行状态：1-成功，2-失败',
  `result` text COMMENT '执行结果',
  `error_msg` text COMMENT '错误信息',
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `update_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `is_deleted` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `idx_task_id` (`task_id`),
  KEY `idx_execute_time` (`execute_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='任务执行日志表';
```

---

## 四、单元测试要求

### 4.1 强制测试场景

根据后端规范第 12 节，Report 和 Ops 模块不涉及钱逻辑，不强制单元测试，但以下场景建议编写测试：

#### Report 模块建议测试：

1. **SQL 注入防护测试**：
   - 参数名包含非法字符应拒绝
   - SQL 包含危险关键字应拒绝

2. **参数化查询测试**：
   - 参数正确绑定
   - 参数值为 null 时处理正确

3. **数据权限测试**：
   - 无权限用户访问报表应拒绝
   - 数据范围过滤正确

#### Ops 模块建议测试：

1. **Cron 表达式验证测试**：
   - 合法 Cron 表达式通过
   - 非法 Cron 表达式拒绝

2. **任务状态流转测试**：
   - 任务启动/暂停状态正确
   - 任务立即执行成功

---

## 五、开发检查清单

### 5.1 Report 模块检查清单

#### 实体类设计

- [ ] 所有实体类继承 `BaseEntity`
- [ ] 表名使用模块前缀 `rpt_`
- [ ] 所有字段添加中文注释
- [ ] 主键使用 GUID
- [ ] 状态字段使用 `int` 类型

#### Service 层实现

- [ ] 所有公开方法添加中文 XML 注释
- [ ] 参数验证使用 DataAnnotations
- [ ] 业务异常使用 `BusinessException`
- [ ] SQL 注入防护实现完整
- [ ] 密码加密存储
- [ ] 数据权限过滤实现

#### Controller 层实现

- [ ] 路由遵循 RESTful 风格
- [ ] 权限标注 `[Permission]`
- [ ] 操作日志标注 `[OperateLog]`
- [ ] Swagger 注释完整
- [ ] 统一响应格式 `ApiResponse<T>`

#### 安全检查

- [ ] SQL 参数化查询
- [ ] 参数名白名单验证
- [ ] SQL 关键字黑名单检测
- [ ] 密码脱敏返回
- [ ] 数据权限控制

### 5.2 Ops 模块检查清单

#### 实体类设计

- [ ] 所有实体类继承 `BaseEntity`
- [ ] 表名使用模块前缀 `ops_`
- [ ] 所有字段添加中文注释
- [ ] 主键使用 GUID
- [ ] 状态字段使用 `int` 类型

#### Service 层实现

- [ ] 所有公开方法添加中文 XML 注释
- [ ] Cron 表达式验证
- [ ] Quartz 任务调度集成
- [ ] 日志清理定时任务

#### Controller 层实现

- [ ] 路由遵循 RESTful 风格
- [ ] 权限标注
- [ ] 操作日志标注
- [ ] Swagger 注释完整

#### 日志功能

- [ ] 操作日志自动记录（ActionFilter）
- [ ] 登录日志记录
- [ ] 任务执行日志记录
- [ ] 日志清理策略

### 5.3 数据库检查清单

- [ ] 表名使用模块前缀 + snake_case
- [ ] 字段名使用 snake_case
- [ ] 所有表和字段添加中文注释
- [ ] 主键使用 GUID
- [ ] 外键约束正确
- [ ] 索引创建合理
- [ ] 字符集 utf8mb4

---

## 六、附录

### 6.1 常用 Cron 表达式

| 表达式 | 说明 |
|--------|------|
| `0 */5 * * * ?` | 每 5 分钟执行一次 |
| `0 0 * * * ?` | 每小时执行一次 |
| `0 0 9 * * ?` | 每天 9 点执行 |
| `0 0 2 * * ?` | 每天凌晨 2 点执行 |
| `0 0 9 * * MON` | 每周一 9 点执行 |
| `0 0 9 1 * ?` | 每月 1 日 9 点执行 |

### 6.2 报表分类建议

| 分类 | 说明 |
|------|------|
| `sales` | 销售报表 |
| `inventory` | 库存报表 |
| `finance` | 财务报表 |
| `customer` | 客户报表 |
| `marketing` | 营销报表 |

### 6.3 操作日志模块对照表

| Module | Target | Action | 说明 |
|--------|--------|--------|------|
| `basic` | `user` | `create/update/delete` | 用户管理 |
| `crm` | `customer` | `create/update/delete` | 客户管理 |
| `product` | `spu` | `create/update/delete` | 商品管理 |
| `mall` | `order` | `create/cancel` | 订单管理 |
| `report` | `definition` | `create/execute/export` | 报表管理 |
| `ops` | `task` | `create/start/pause` | 任务管理 |

---

> 本文档与 `backend-guidelines.md`、`api/other-modules.md` 共同构成 Report 和 Ops 模块的开发依据。开发过程中如有疑问，优先参考本说明，其次参考后端规范和接口定义。

