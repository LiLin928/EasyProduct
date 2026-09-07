# EasyProduct 后端开发规范

> **适用对象**：EasyProduct.WebApi（.NET 8 模块化单体后端）
> **上游依据**：`docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md`（整合设计方案）
> **对齐文档**：`docs/frontend-guidelines.md`（前端规范，数据契约以其 1.1/1.2 节为准，后端必须严格对齐）
> **来源**：EasyWechatWeb（主要继承源）、WebSiteNewBack（仅参考，代码不迁入）、EasyCRM（无后端）

---

## 1. 适用范围与定位

- 本规范约束 EasyProduct.WebApi 解决方案内所有 C# 代码：目录结构、分层、API 设计、数据访问、异常日志、测试与工程门禁。
- 与源项目旧写法冲突时，**以本规范为准**；差异裁决记录见附录 A。
- 规范未覆盖的问题，优先参考 EasyWechatWeb 现有实现，其次在本规范精神内自行裁量并在 PR/提交说明中注明。

## 2. 技术栈基线（锁定版本）

| 类别 | 选型 | 基线版本 | 说明 |
|------|------|---------|------|
| 运行时 | .NET | 8.0 (LTS) | 不升级、不预览版 |
| ORM | SqlSugarCore | 5.1.4.x | 继承 EasyWechatWeb |
| MySQL 驱动 | MySqlConnector | 2.5.x | **统一用 MySqlConnector，移除 MySql.Data**（两者并存是历史遗留） |
| DI 容器 | Autofac + Autofac.Extensions.DependencyInjection | 8.x / 9.x | 程序集扫描约定注册 |
| 日志 | Serilog（Console + 滚动 File） | 8.x | 不接 ES |
| 对象映射 | Mapster | 10.x | 不用 AutoMapper |
| 认证 | Microsoft.AspNetCore.Authentication.JwtBearer | 8.x | 统一用 JwtBearer 签发/校验；不引入第三方 JWT 库 |
| API 文档 | Swashbuckle.AspNetCore | 6.9.x | Swagger/OpenAPI |
| 定时任务 | Quartz | 3.14.x | Ops 模块统一调度 |
| 密码哈希 | BCrypt.Net-Next | 4.x | 继承现有实现 |
| Excel | MiniExcel | 1.34.x | **不用 NPOI**（历史遗留统一替换） |
| 缓存 | StackExchange.Redis + Microsoft.Extensions.Caching.Memory | 2.8.x / 8.x | Redis 可选，关闭自动降级内存缓存 |

**明确不引入**（YAGNI，与整合设计 11.1 一致）：

EF Core、Dapper、FluentValidation、AutoMapper、DotNetCore.CAP（含 Kafka/MySql 模式）、Elasticsearch 客户端、Minio 客户端（存储抽象接口保留，实现仅本地）、NPOI、ApiVersioning、MediatR、Hangfire。

## 3. 解决方案结构与模块边界

```text
EasyProduct.WebApi/
├─ EasyProduct.sln
├─ EasyProduct.Web/                 # ASP.NET Core 宿主
│  ├─ Controllers/
│  │  ├─ Admin/<模块>/              # /api/admin/**  管理端（AdminJwt）
│  │  ├─ Site/<模块>/               # /api/site/**   官网公开（匿名+限流）
│  │  └─ App/<模块>/                # /api/app/**    小程序会员（MemberJwt）
│  ├─ Middleware/                   # 全局异常、请求日志、分区守卫、限流
│  ├─ Filters/                      # 操作日志等 Action 级切面
│  ├─ Program.cs / appsettings*.json
├─ EasyProduct.Business/            # 业务层，八大模块各一个目录
│  ├─ Basic/  Site/  Product/  Mall/  Crm/  Workflow/  Report/  Ops/
│  │  └─ 每模块内：I<Xxx>Service.cs + <Xxx>Service.cs（平铺，不再嵌 IService/Service 两层目录）
├─ EasyProduct.Models/              # 实体/DTO/Options/常量
│  ├─ Entitys/<模块>/               # SqlSugar 实体
│  ├─ Dto/<模块>/                   # 入参/出参 DTO
│  ├─ Options/                      # 配置绑定类
│  └─ Constants/                    # 字符串常量（状态值等，见 9.5）
├─ EasyProduct.Common/              # 通用组件（继承 CommonManager 精华）
│  ├─ Base/                         # ApiResponse、PageResponse、BaseController、BaseService
│  ├─ Attributes/  Cache/  Error/  Extensions/  Helper/  Logging/  SqlSugar/
└─ EasyProduct.Tests/               # xUnit 测试工程
```

**模块边界（强制）：**

1. 八大业务模块（Basic/Site/Product/Mall/Crm/Workflow/Report/Ops）之间**只允许通过对方模块的 Service 接口调用**，禁止跨模块直接 `Queryable` 对方的实体表。
2. 依赖方向单向：`Web → Business → Common/Models`；`Business` 模块之间平级调用，禁止形成环（出现环说明边界划错，提交设计评审）。
3. 同一资源对三端暴露时，Controller 按分区各写一个（如商品：`Admin/Product/ProductController` 管理 + `App/Product/ProductController` 会员视角），**共用同一个 Service**，不允许把业务逻辑写进 Controller。

## 4. 分层与依赖注入

### 4.1 分层职责

| 层 | 职责 | 禁止 |
|----|------|------|
| Controller | 收参、调用 Service、返回统一信封；鉴权特性标注 | 业务逻辑、直接访问 `_db`、try/catch（见 8.2） |
| Service | 业务规则、事务、DTO↔实体映射、跨模块调用 | 感知 HTTP（不引用 HttpContext/Controller 基类成员） |
| Common/Helper | 无状态工具 | 引用 Business |

### 4.2 依赖注入（裁决：Autofac 扫描 + 构造器注入）

1. **一律构造器注入**。EasyWechatWeb 的属性注入写法（`public IXxxService _xxx { get; set; } = null!;`）在迁入时全部改写。
2. `BaseService` 调整为构造器持有数据库上下文：

```csharp
public abstract class BaseService
{
    protected ISqlSugarClient _db { get; }
    protected BaseService(ISqlSugarClient db) => _db = db;
}

public class CustomerService : BaseService, ICustomerService
{
    private readonly ILogger<CustomerService> _logger;
    public CustomerService(ISqlSugarClient db, ILogger<CustomerService> logger) : base(db)
    {
        _logger = logger;
    }
}
```

3. Autofac 按命名约定批量注册：类名以 `Service` 结尾 → `AsImplementedInterfaces()` + `InstancePerLifetimeScope`；以 `SingletonService` 结尾或显式标注 `[SingletonService]` → 单例。特例（需要属性/装饰器注入的）在 `Program.cs` 显式注册并注明原因。
4. Controller 通过构造器注入 Service；`BaseController` 不持有业务依赖。
5. 禁止 Service Locator（运行时从容器手动 Resolve）——单例内需要 scoped 服务时用 `IServiceScopeFactory` 并集中在一处（Quartz Job 基类）。

## 5. API 设计约定（与前端规范严格对齐）

### 5.1 三分区路由

| 分区 | 路由前缀 | 认证 | 消费者 |
|------|---------|------|--------|
| 管理端 | `/api/admin/<模块>/<资源>` | AdminJwt + 权限标识 | EasyProduct.Admin |
| 官网公开 | `/api/site/<资源>` | 匿名（提交类接口 IP 限流） | EasyProduct.Site |
| 小程序 | `/api/app/<模块>/<资源>` | MemberJwt | EasyProduct.MiniApp |

Controller 路由模板（管理端示例）：

```csharp
[ApiController]
[Route("api/admin/crm/customer")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class CustomerController : BaseController { ... }
```

### 5.2 资源风格动作（裁决：废弃 `[action]` 路由，对齐前端规范 1.2）

| 动作 | HTTP | 路由 | 入参 | 返回 data |
|------|------|------|------|-----------|
| 分页列表 | GET | `<资源>/list` | query：`pageIndex/pageSize/keyword` + 业务筛选 | `PageResponse<T>` |
| 详情 | GET | `<资源>/{id}` | 路径 GUID | `T` |
| 创建 | POST | `<资源>` | body DTO | 新记录 GUID（string） |
| 更新 | PUT | `<资源>/{id}` | body DTO | `true` |
| 删除 | DELETE | `<资源>/{id}` | 路径 GUID（软删） | `true` |
| 不分页全量 | GET | `<资源>/all` | 可选筛选 | `List<T>` |
| 复杂查询逃生舱 | POST | `<资源>/query` | body 查询 DTO | `PageResponse<T>` |

补充规则：

1. 资源名用**小写连字符单数名词**（`customer`、`tax-rate`），与前端 `api/<域>/<模块>.ts` 一一对应。
2. 状态切换等子操作用 `PUT <资源>/{id}/status`，body `{ "status": "enabled" }`。
3. 批量操作：`POST <资源>/batch-delete`，body `{ "ids": [...] }`（批量属动作，允许动词）。
4. **分页参数固定 `pageIndex`（从 1 起）/ `pageSize`（默认 10，上限 200）**；EasyCRM 旧 mock 的 `pageNum` 在迁入页面时一并改写。
5. 不引入 API 版本号；破坏性变更通过新增资源/字段（向后兼容）解决，确实无法兼容时在整合设计评审中决策。

### 5.3 统一响应信封（HTTP 一律 200）

所有接口（含错误）HTTP 状态码返回 200，业务结果在信封内表达——与前端拦截器"只看 `code`"的约定严格一致：

```json
{ "code": 200, "message": "操作成功", "data": { }, "timestamp": 1754649600000 }
```

| code | 语义 | 触发 |
|------|------|------|
| 200 | 成功 | 正常返回 |
| 400 | 参数错误/业务规则不满足 | ModelState 校验失败、`BusinessException.BadRequest` |
| 401 | 未登录/Token 失效 | JWT 校验失败、Token 过期、黑名单命中 |
| 403 | 无权限/身份分区不匹配 | 权限标识校验失败、会员 Token 调管理接口 |
| 404 | 资源不存在 | `BusinessException.NotFound` |
| 500 | 服务器内部错误 | 未捕获异常（message 固定"系统繁忙，请稍后重试"，详情只进日志） |

两个定制点（P0 脚手架落实）：

1. `JwtBearerEvents.OnChallenge`：拦截默认 401 challenge，改写信封 `{code:401,...}`，HTTP 仍为 200。
2. `InvalidModelStateResponseFactory`：模型校验失败写信封 `{code:400, message:第一条错误, data:null}`。

分页出参 `PageResponse<T>`（`list/total` 为前端必需字段，其余为兼容附加字段，前端可忽略）：

```json
{ "list": [], "total": 0, "pageIndex": 1, "pageSize": 10, "totalPages": 0, "hasNextPage": false, "hasPrevPage": false }
```

### 5.4 Swagger 与接口文档

1. 所有 Controller/Action 必须写 XML 注释（`summary` + 关键 `remarks`），并标注 `[ProducesResponseType(typeof(ApiResponse<T>), 200)]`。
2. Development 环境常开 Swagger UI；`IsUseSwagger=true` 时启用。
3. 每个模块完成后导出 `swagger.json` 归档（`docs/api/`），供前端与 mock 对照（见 mock 规范第 9 节）。
### 5.5 限流与幂等

- `/api/site/**` 提交类接口（询价、联系表单）：按 IP 限流（默认 10 次/分钟，配置可调），超限返回信封 `code=400, message="提交过于频繁"`（HTTP 200）——不引入前端未定义的新 code。
- 支付回调等幂等接口：以业务单号做幂等键（Redis SETNX 或唯一索引），重复请求直接返回首次结果。

---

## 6. 认证与权限

### 6.1 双身份双 Scheme（与整合设计 5.1 一致）

| Scheme | 身份 | Claims | 登录方式 |
|--------|------|--------|---------|
| `AdminJwt` | `basic_user` 管理端用户 | `UserId`、`UserName`、`RealName`、`Roles`、`identity_type=admin`、`jti` | 账号+密码（BCrypt） |
| `MemberJwt` | `mall_member` 小程序会员 | `UserId`、`OpenId`、`identity_type=member`、`jti` | 微信授权（code 换 openid） |

1. 两类用户**永不共表**；Token 签发密钥可用同一配置节但 `issuer/audience` 区分。
2. **分区守卫中间件**（`ApiZoneGuardMiddleware`）：`/api/admin/**` 要求 `identity_type=admin`；`/api/app/**` 要求 `identity_type=member`；不匹配返回信封 403。杜绝会员 Token 调管理接口。
3. 双 Token 机制继承 EasyProject：`accessToken`（短时效）+ `refreshToken`（长时效），刷新接口 `POST /api/admin/auth/refresh`；登出将 `jti` 写入黑名单。
4. **Redis 关闭时降级**：单 access Token、无刷新、无黑名单（由配置开关控制，继承设计决策）。

### 6.2 权限模型（菜单 RBAC + 权限标识）

1. 权限标识格式：`模块:页面:操作`（如 `crm:customer:edit`），与前端规范 2.6、菜单种子数据一致。
2. 接口标注自定义特性：`[Permission("crm:customer:edit")]`，底层注册为 Policy；超级管理员角色短路放行。
3. 数据权限本期仅两档：**本人 / 本部门**（查询时按 `CreateBy`/部门字段过滤，Service 层统一处理），不做集团多组织。
4. `BaseController` 提供 `GetCurrentUserId()/GetCurrentUserName()/GetCurrentRealName()/IsAdmin()`，Controller 不得自行解析 Token。

---

## 7. 数据访问约定（SqlSugar）

### 7.1 库与命名

1. 单库 `easyproduct`，utf8mb4；表名 = 模块前缀 + snake_case（`basic_user`、`crm_customer`、`mall_order_item`）。
2. 实体类 PascalCase 属性，数据库列名统一 snake_case：通过 SqlSugar 全局 `ConfigureExternalServices.EntityService` 做 PascalCase→snake_case 映射，**实体属性不手写列名**（主键、特殊长度等除外用 `[SugarColumn]`）。
3. 实体必须标注 `[SugarTable("crm_customer", "客户表")]`（表名 + 中文描述）、列用 `ColumnDescription` 写中文注释——建库脚本由 CodeFirst 生成后人工校对归档到 `sql/`。

### 7.2 实体基类（所有业务表继承）

```csharp
public abstract class BaseEntity
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();          // GUID 主键，禁止自增
    public DateTime CreateTime { get; set; }                 // 创建时间（服务器时间）
    public DateTime UpdateTime { get; set; }                 // 更新时间（保存前统一刷新）
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? CreateBy { get; set; }                    // 创建人（管理端=用户名；系统任务=system）
    public bool IsDeleted { get; set; }                      // 软删标记
}
```

- `UpdateTime` 由 SqlSugar AOP（`DataExecuting`）或 BaseService 保存方法统一维护，业务代码不手写。
- 涉及多端修改的表可增加 `UpdateBy`，按需评估。

### 7.3 查询与写入

1. 优先 LINQ（`Queryable`/`Insertable`/`Updateable`/`Deleteable`）；多表关联用 `JoinQueryInfos`，**禁止为图省事写大段裸 SQL**。
2. 确需裸 SQL（复杂报表）时：必须参数化（`new SugarParameter`），禁止字符串拼接；且只能出现在报表（Report）与本模块内。
3. **软删全局过滤器**：`QueryFilter.AddTableFilter<ISoftDelete>(x => !x.IsDeleted)`；需要查已删数据的场景显式 `ClearFilter` 并在代码注释说明。
4. 删除操作一律软删；物理删除仅限日志清理类定时任务。
5. 跨模块取数走对方 Service 接口（见 3），禁止 join 对方表。

### 7.4 事务与涉钱操作

1. 单服务内多表写入用 `_db.Ado.UseTranAsync`（或 `BeginTran/Commit/Rollback`）；跨 Service 的编排事务由发起方 Service 持有事务并调用对方 Service 方法。
2. **涉钱三处强制事务 + 强制单测**（整合设计 12 节）：库存流水（`crm_stock_record`）、冲销（`crm_reversal`）、收付款核销（`crm_payment`/核销记录）。先写 xUnit 再改实现。
3. 金额字段 `decimal(18,2)`：实体标注 `[SugarColumn(Length = 18, DecimalDigits = 2)]`；禁止 `double/float` 存金额。

### 7.5 时间与枚举值

1. 时间字段数据库 `datetime`，JSON 序列化输出 **ISO 8601**（`2026-08-08T10:00:00`）；全局 `JsonSerializerOptions` 不特殊处理 DateTime（默认即 ISO）。
2. **业务状态/类型字段统一使用 `int` 类型**，与前端常量、`basic_dict` 字典值映射一致。枚举定义在 `EasyProduct.Models/Enums/`，值与前端 `as const` 常量逐字一致。
3. **布尔字段统一使用 `int` 类型**（0=false，1=true），不使用数据库 `BOOLEAN` 类型。

**详细规范见：** `docs/superpowers/specs/2026-09-07-status-enum-standardization-design.md`

#### 7.5.1 状态枚举定义

```csharp
/// <summary>
/// 通用状态枚举
/// </summary>
public enum Status
{
    /// <summary>禁用</summary>
    Disabled = 0,
    /// <summary>启用</summary>
    Enabled = 1
}

/// <summary>
/// 订单状态枚举
/// </summary>
public enum OrderStatus
{
    /// <summary>已取消</summary>
    Cancelled = 0,
    /// <summary>待支付</summary>
    Pending = 1,
    /// <summary>已支付</summary>
    Paid = 2,
    /// <summary>已发货</summary>
    Shipped = 3,
    /// <summary>已完成</summary>
    Completed = 4,
    /// <summary>已退款</summary>
    Refunded = 5
}
```

#### 7.5.2 实体字段定义

```csharp
/// <summary>
/// 状态：0=禁用，1=启用
/// </summary>
public Status Status { get; set; }

/// <summary>
/// 是否可见：0=否，1=是
/// </summary>
public int Visible { get; set; }

/// <summary>
/// 订单状态：0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款
/// </summary>
public OrderStatus OrderStatus { get; set; }
```

#### 7.5.3 数据库字段类型

```sql
-- 状态字段
status INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用'

-- 布尔字段
is_default INT DEFAULT 0 COMMENT '是否默认：0=否，1=是'

-- 订单状态
order_status INT DEFAULT 1 COMMENT '订单状态：0=已取消，1=待支付，2=已支付，3=已发货，4=已完成，5=已退款'
```

---

## 8. 异常与日志

### 8.1 业务异常

```csharp
throw BusinessException.BadRequest("客户名称已存在");
throw BusinessException.NotFound("客户不存在");
throw new BusinessException("库存不足，无法出库", 400);
```

- 可预期的业务失败一律抛 `BusinessException`（含 ErrorCode），**不允许用返回 null/魔法值表达失败**。

### 8.2 全局异常中间件（裁决：Controller 禁止手写 try/catch）

`ExceptionHandlingMiddleware`（继承 EasyWechatWeb 实现）统一转换：

| 异常 | 输出 |
|------|------|
| `BusinessException` | 信封 code=其 ErrorCode，message=异常消息 |
| 未捕获异常 | 信封 code=500，message 固定"系统繁忙，请稍后重试"；完整堆栈进 Error 日志（带 TraceId） |

- **Controller/Service 不得再写 `try/catch` 包业务代码**（EasyWechatWeb 每个 Action 手写 try/catch 的写法废弃）。唯一例外：调用第三方 SDK（微信支付回调验签等）需要精细处理失败分支时，允许局部 catch 并转 `BusinessException`。

### 8.3 Serilog 配置

1. Sink：Console + File（`logs/app-yyyyMMdd.log`，按天滚动，保留 30 天）；**不接 ES**（整合设计已裁）。
2. 级别：Development=Debug，Production=Information；Microsoft.* 框架日志压到 Warning。
3. 请求日志中间件记录：method、path、耗时、响应 code、TraceId（`Activity.TraceId` 或自生成 `X-Trace-Id` 透传）。
4. SQL 日志：SqlSugar AOP `OnLogExecuting` 以 Debug 级别输出（生产不开），慢查询（>1s）升 Warning。

### 8.4 操作日志（审计）

- 写操作接口标注 `[OperateLog(Module = "crm", Target = "customer", Action = "create")]`（ActionFilter 继承 EasyWechatWeb `OperateLogActionFilter`），落 `ops_operate_log`：操作人、IP、入参摘要、结果、耗时、TraceId。
- 登录成功/失败落 `ops_login_log`（安全基线，整合设计 5.4）。

---

## 9. DTO 与校验

### 9.1 DTO 命名与组织

| 用途 | 命名 | 示例 |
|------|------|------|
| 分页查询入参 | `XxxQuery` | `CustomerQuery : PageQuery` |
| 复杂查询入参 | `XxxQueryDto` | — |
| 创建入参 | `XxxCreateDto` | `CustomerCreateDto` |
| 更新入参 | `XxxUpdateDto` | `CustomerUpdateDto` |
| 出参 | `XxxDto` | `CustomerDto` |

- `PageQuery` 基类含 `PageIndex=1`、`PageSize=10`（`[Range(1,200)]`）。
- DTO 放在 `EasyProduct.Models/Dto/<模块>/`；**实体不得作为接口入参或出参**（防止字段泄漏与过度绑定）。

### 9.2 校验（裁决：DataAnnotations + Service 业务校验）

1. 格式类校验用特性：`[Required(ErrorMessage="客户名称不能为空")] [MaxLength(50)] [Range] [RegularExpression]`。
2. ModelState 失败由 `InvalidModelStateResponseFactory` 转信封 400（取第一条错误作 message）。
3. 业务规则校验（唯一性、状态流转、余额充足等）在 Service 内执行，抛 `BusinessException`。
4. 不引入 FluentValidation。

### 9.3 映射

- Mapster 全局 `TypeAdapterConfig` 在启动时注册（需要自定义规则的映射集中配置）；Service 内用 `dto.Adapt<Customer>()` / `entity.Adapt<CustomerDto>()`。
- 映射规则随 DTO 所在模块维护，禁止散落在 Controller。
---

## 10. 缓存、文件与定时任务

### 10.1 缓存

1. 统一走 `ICacheService` 抽象（继承 EasyWechatWeb `AddCacheService`）：配置 `Cache:Provider=redis|memory`，Redis 关闭自动降级 `IMemoryCache`，业务代码不感知。
2. Key 规范：`{模块}:{业务}:{标识}`（`basic:dict:order_status`、`jwt:blacklist:{jti}`），过期时间显式声明，禁止永不过期的大对象。
3. 字典、菜单树等低频变更数据优先缓存，写操作时主动失效。

### 10.2 文件存储

1. 一律经 `IFileStorageHelper` 抽象（当前仅本地实现；MinIO 接口预留不启用）。
2. 本地存储根目录 `uploads/`，对外路径 `/uploads/{module}/{yyyyMMdd}/{guid}{ext}`；静态文件中间件挂载规则继承 Program.cs 现状。
3. 上传白名单：图片（jpg/png/webp/gif）、文档（pdf/doc/docx/xls/xlsx）、压缩包（zip），单文件 ≤10MB（配置可调）；非法类型返回信封 400。

### 10.3 定时任务（Quartz）

1. Job 放 `EasyProduct.Business/Ops/Jobs/`，实现 `IJob`，类名 `XxxJob`；cron 表达式配在 `appsettings` `QuartzJobs` 节，支持按环境启停。
2. 每次执行写 `ops_job_log`（任务名、参数、开始/结束、耗时、成功/失败、异常摘要），失败自动告警日志（Error 级）。
3. Job 内需要 scoped 服务时用 `IServiceScopeFactory` 创建作用域（Quartz Job 基类统一封装，见 4.2-5）。
4. 本期任务清单：订单超时自动取消（Mall）、库存上下限预警扫描（Crm）、语言包远程缓存刷新（Basic，配合前端规范 1.4）。

---

## 11. 配置与环境

1. `appsettings.json`（基线）+ `appsettings.Development.json` / `appsettings.Production.json`；敏感值（数据库口令、JWT Secret、微信密钥）**不入库**，用环境变量注入（`ConnectionStrings__Default` 等），仓库提供 `appsettings.Development.example.json`。
2. 配置绑定用 Options 模式（`EasyProduct.Models/Options/`），禁止在业务代码里散落 `configuration["xxx"]`。
3. 环境划分：Development（本地，Swagger 开、SQL 日志开）/ Production（Docker Compose）。
4. 健康检查：`/health`（P0 交付），供 nginx/运维探活。

## 12. 测试约定

1. 测试工程 `EasyProduct.Tests`（xUnit），命名 `<类名>Tests`，方法名 `<方法>_<场景>_<期望>`，given-when-then 分段注释。
2. **强制覆盖三处涉钱逻辑**（先测后码）：库存流水、冲销、收付款核销；覆盖正例 + 至少 2 个边界（余额不足、并发重复提交）。
3. 其余模块不强制单测，靠 Swagger 手工冒烟 + 前端联调（整合设计 12 节轻量策略）。
4. 测试用独立 MySQL 库（或 SqlSugar 指向测试库），不使用生产连接串；测试数据自建自清。

## 13. 工程门禁与提交

1. `dotnet build` 必须 0 错误；警告应清零（新增代码不得引入新警告）。
2. 提交前跑 `dotnet build` + `dotnet test`（有测试改动时）。
3. Commit 遵循 Conventional Commits，scope 用 `api`（后端），示例：`feat(api): CRM 客户主档 CRUD`、`fix(api): 库存流水事务回滚遗漏`。
4. 每个功能模块完成 = 代码 + Swagger 注释 + （涉钱则）单测 + `docs/api/swagger-<模块>.json` 归档。

---

## 附录 A：三源项目差异裁决记录

| # | 事项 | EasyWechatWeb 现状 | WebSiteNewBack | EasyCRM | **裁决** |
|---|------|------|------|------|------|
| 1 | 响应信封 | `{code,message,data,timestamp}` | `{data,success}` | mock 同 EasyProject | **统一 `{code,message,data,timestamp}`**；HTTP 一律 200 |
| 2 | 路由风格 | `[Route("api/[controller]/[action]")]` + POST 居多 | 类同 | RESTful（GET/POST/PUT/DELETE 资源式） | **RESTful 资源式**（前端规范 1.2 已锁定） |
| 3 | 分页入参 | `PageIndex` | — | `pageNum` | **`pageIndex/pageSize`**（前端规范已锁定；CRM 页面迁入时改写） |
| 4 | DI 注入 | Autofac + 属性注入 | Autofac + 属性注入 | — | **Autofac 扫描 + 构造器注入**（本次决策 2） |
| 5 | Controller try/catch | 每个 Action 手写 | 同 | — | **禁止**，全局异常中间件统一处理（本次裁决） |
| 6 | CAP 事件总线 | CAP + Kafka/MySql | — | — | **裁掉**（本次决策 4），模块协作走同步调用+事务 |
| 7 | 日志 ES Sink | Serilog→ES | — | — | **裁掉**，Console + 滚动文件 |
| 8 | MySQL 驱动 | MySql.Data + MySqlConnector 并存 | MySql.Data | — | **仅 MySqlConnector** |
| 9 | Excel 库 | MiniExcel | — | — | **MiniExcel**（设计文档提及的 NPOI 统一替换） |
| 10 | 状态字段 | 部分数字码 | — | 字符串 | **int 类型枚举**（本次决策 7.5），与前端常量、basic_dict 映射一致 |
| 11 | 表名 | PascalCase（`User`） | PascalCase | — | **模块前缀 + snake_case**（整合设计 6.1） |
| 12 | 校验 | 手工 if + BusinessException | 同 | — | **DataAnnotations + Service 业务校验**（本次决策 3） |
| 13 | JWT 库 | JwtBearer + 第三方 JWT 包并存 | JwtBearer | — | **仅 JwtBearer 体系** |

---

> 本规范与整合设计方案、前端规范构成 EasyProduct 三份基线文档。后端实现细节（如具体表结构）随 P1~P5 阶段推进，由 writing-plans 产出的实施计划细化；本规范如需修订，走「修订 → 自审 → 用户评审 → 提交」流程。