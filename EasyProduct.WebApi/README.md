# EasyProduct.WebApi 后端基础架构

## 项目概述

EasyProduct.WebApi 是基于 .NET 8 的模块化单体后端 API，遵循 Clean Architecture 原则设计。

## 技术栈

| 组件 | 版本 | 说明 |
|------|------|------|
| .NET | 8.0 (LTS) | 运行时 |
| SqlSugarCore | 5.1.4 | ORM |
| MySqlConnector | 2.5.x | MySQL 驱动 |
| Autofac | 8.x | DI 容器 |
| Serilog | 8.x | 日志框架 |
| Mapster | 10.x | 对象映射 |
| Swashbuckle | 6.9.x | Swagger/OpenAPI |
| BCrypt.Net-Next | 4.x | 密码哈希 |
| MiniExcel | 1.34.x | Excel 处理 |
| Quartz | 3.14.x | 定时任务 |
| xUnit | 2.9.x | 单元测试 |

## 项目结构

`
EasyProduct.WebApi/
├── EasyProduct.sln                 # 解决方案文件
├── EasyProduct.Web/              # Web API 宿主项目
│   ├── Controllers/
│   │   ├── Admin/               # 管理端 API (/api/admin/**)
│   │   │   ├── Base/            # 管理端控制器基类
│   │   │   └── Basic/           # 基础管理模块控制器
│   │   ├── App/                 # 小程序端 API (/api/app/**)
│   │   │   └── Base/            # 小程序端控制器基类
│   │   ├── Site/                # 官网端 API (/api/site/**)
│   │   │   └── Base/            # 官网端控制器基类
│   │   └── HealthController.cs  # 健康检查端点
│   ├── Middleware/              # 中间件
│   │   ├── ExceptionHandlingMiddleware.cs  # 全局异常处理
│   │   └── RequestLoggingMiddleware.cs     # 请求日志
│   ├── Filters/                 # 过滤器
│   ├── appsettings.json         # 主配置
│   ├── appsettings.Development.json  # 开发环境配置
│   └── Program.cs               # 入口程序
├── EasyProduct.Business/         # 业务层
│   ├── Basic/                   # 基础管理模块
│   │   ├── IUserService.cs      # 服务接口
│   │   └── UserService.cs       # 服务实现
│   ├── Site/                    # 官网模块
│   ├── Product/                 # 商品模块
│   ├── Mall/                    # 商城模块
│   ├── Crm/                     # 客户关系模块
│   ├── Workflow/                # 工作流模块
│   ├── Report/                  # 报表模块
│   └── Ops/                     # 运维模块
├── EasyProduct.Models/         # 模型层
│   ├── Entitys/                 # 数据库实体
│   │   └── Basic/basic_user.cs  # 用户实体
│   ├── Dto/                     # DTO
│   │   └── Basic/UserDto.cs     # 用户 DTO
│   ├── Options/                 # 配置绑定类
│   └── Constants/               # 常量
├── EasyProduct.Common/         # 通用组件层
│   ├── Base/                    # 基础类
│   │   ├── ApiResponse.cs       # 统一响应格式
│   │   ├── PageResponse.cs      # 分页响应格式
│   │   ├── BaseController.cs    # 控制器基类
│   │   └── BaseService.cs       # 服务基类
│   ├── Error/                   # 异常类
│   │   └── BusinessException.cs # 业务异常
│   ├── Extensions/              # 扩展方法
│   │   ├── AutofacModuleRegister.cs  # Autofac 注册
│   │   └── SqlSugarExtensions.cs     # SqlSugar 扩展
│   ├── Logging/                 # 日志配置
│   │   └── SerilogConfiguration.cs
│   ├── Cache/                   # 缓存
│   ├── Helper/                  # 工具类
│   ├── Attributes/              # 自定义特性
│   └── SqlSugar/                # SqlSugar 相关
└── EasyProduct.Tests/           # 测试项目
    └── EasyProduct.Tests.csproj
`

## 快速开始

### 1. 还原 NuGet 包

`bash
cd EasyProduct.WebApi
dotnet restore
`

### 2. 配置数据库连接

编辑 EasyProduct.Web/appsettings.Development.json：

`json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Port=3306;Database=easyproduct_dev;Uid=root;Pwd=your_password;Charset=utf8mb4;SslMode=None;"
  }
}
`

### 3. 运行项目

`bash
cd EasyProduct.Web
dotnet run
`

### 4. 访问 Swagger 文档

打开浏览器访问：http://localhost:5000/swagger

## 统一响应格式

### 成功响应

`json
{
  "code": 200,
  "message": "操作成功",
  "data": { ... },
  "timestamp": 1704067200000
}
`

### 失败响应

`json
{
  "code": 400,
  "message": "参数错误",
  "data": null,
  "timestamp": 1704067200000
}
`

### 分页响应

`json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [ ... ],
    "total": 100,
    "pageIndex": 1,
    "pageSize": 10,
    "totalPages": 10,
    "hasNextPage": true,
    "hasPrevPage": false
  },
  "timestamp": 1704067200000
}
`

## API 路由规范

| 端点 | 路由前缀 | 认证 |
|------|----------|------|
| 管理端 | /api/admin/** | Admin JWT |
| 小程序端 | /api/app/** | Member JWT |
| 官网端 | /api/site/** | 匿名（+限流）|
| 健康检查 | /api/health | 无 |

## 开发规范

### Controller 层

- 继承对应的基类：AdminControllerBase、AppControllerBase、SiteControllerBase
- **禁止**在 Controller 中写业务逻辑
- **禁止**使用 try/catch，由全局异常中间件处理

### Service 层

- 继承 BaseService 获得基础 CRUD 方法
- 所有公共方法必须添加中文注释
- 使用构造器注入获取依赖
- 业务异常使用 BusinessException 抛出

### Entity 层

- 使用 SqlSugar 的 [SugarTable] 特性标注表名
- 表名使用 snake_case 命名（模块前缀 + 表名）
- **必须继承 BaseEntity**，自动包含通用字段（Id、IsDeleted、Status、CreatedAt、UpdatedAt 等）
- IsDeleted 使用 **int 类型**（0=未删除，1=已删除），**不使用 bool**
- GUID 作为主键

## 代码示例

### 创建 Entity

`csharp
using SqlSugar;
using EasyProduct.Models.Entitys.Base;

namespace EasyProduct.Models.Entitys.Basic;

/// <summary>
/// 用户实体
/// </summary>
/// <remarks>
/// 继承 BaseEntity，自动包含：Id、IsDeleted（int）、Status、CreatedAt、UpdatedAt 等字段
/// </remarks>
[SugarTable("basic_user", "用户表")]
public class basic_user : BaseEntity
{
    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 50)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 用户状态（active/inactive）
    /// </summary>
    [SugarColumn(Length = 20)]
    public string Status { get; set; } = "active";
}
`

**BaseEntity 字段说明：**
- `Id`: GUID 主键
- `IsDeleted`: **int 类型**（0=未删除，1=已删除）
- `Status`: Status 枚举（Disabled=0，Enabled=1）
- `CreatedAt`: 创建时间
- `UpdatedAt`: 更新时间（可空）
- `CreatedBy`: 创建人ID（可空）
- `UpdatedBy`: 更新人ID（可空）

### 创建 Service

`csharp
using EasyProduct.Common.Base;
using EasyProduct.Models.Entitys.Basic;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 分页查询用户列表
    /// </summary>
    Task<PageResponse<UserDto>> GetPageListAsync(UserQueryDto query);
}

/// <summary>
/// 用户服务实现
/// </summary>
public class UserService : BaseService, IUserService
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public UserService()
    {
    }

    /// <summary>
    /// 分页查询用户列表
    /// </summary>
    public async Task<PageResponse<UserDto>> GetPageListAsync(UserQueryDto query)
    {
        var queryable = _db.Queryable<basic_user>()
            .Where(u => u.IsDeleted == 0);  // 使用 int 类型：0=未删除

        // 关键词搜索
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            queryable = queryable.Where(u => u.UserName.Contains(query.Keyword));
        }

        // 分页查询
        return await queryable
            .OrderBy(u => u.CreatedAt, OrderByType.Desc)
            .ToPageAsync(query.PageIndex, query.PageSize);
    }
}
`

### 创建 Controller

`csharp
using EasyProduct.Web.Controllers.Admin.Base;

namespace EasyProduct.Web.Controllers.Admin.Basic;

/// <summary>
/// 用户管理控制器
/// </summary>
public class UserController : AdminControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 分页查询用户列表
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PageResponse<UserDto>>> GetPageList([FromQuery] UserQueryDto query)
    {
        var result = await _userService.GetPageListAsync(query);
        return Success(result);
    }
}
`

## 提交规范

使用 Conventional Commits 规范：

`
feat(api): 添加用户管理功能
fix(api): 修复分页查询 bug
refactor(api): 重构用户服务
`

## 相关文档

- [后端开发规范](/docs/backend-guidelines.md)
- [前端开发规范](/docs/frontend-guidelines.md)
- [整合设计方案](/docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md)