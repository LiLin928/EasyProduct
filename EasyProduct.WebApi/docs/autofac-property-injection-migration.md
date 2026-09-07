# Autofac 属性注入迁移指南

> 基于 EasyWechatWeb 项目实现，简化 Controller 和 Service 的依赖注入

---

## ✅ 已完成的修改

### 1. AutofacModuleRegister.cs

**修改内容：**
- 为所有注册类型添加 `.OnActivated()` 钩子
- 新增 `InjectProperties()` 方法，自动注入以 `_` 开头的公共属性

**核心实现：**
```csharp
// 注册服务时添加 OnActivated
builder.RegisterAssemblyTypes(assemblies.ToArray())
    .Where(t => t.Name.EndsWith("Service") && !t.IsAbstract && !t.IsInterface)
    .AsImplementedInterfaces()
    .AsSelf()
    .InstancePerLifetimeScope()
    .OnActivated(e =>
    {
        InjectProperties(e.Instance, e.Context);
    });

// 手动注入属性（自动注入以 _ 开头的公共属性）
private void InjectProperties(object instance, IComponentContext context)
{
    var type = instance.GetType();
    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => p.Name.StartsWith("_") && p.CanWrite);

    foreach (var property in properties)
    {
        if (property.GetValue(instance) != null)
            continue;

        var serviceType = property.PropertyType;
        if (context.TryResolve(serviceType, out var service))
        {
            property.SetValue(instance, service);
        }
    }
}
```

---

### 2. UserController.cs

**修改前：**
```csharp
public class UserController : AdminControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
}
```

**修改后：**
```csharp
public class UserController : AdminControllerBase
{
    /// <summary>
    /// 用户服务接口（属性注入）
    /// </summary>
    public IUserService _userService { get; set; } = null!;
}
```

**变化：**
- ❌ 删除了 `private readonly` 字段
- ❌ 删除了构造函数
- ✅ 改为 `public` 属性
- ✅ 添加 `= null!` 非空断言
- ✅ 添加 XML 注释说明"属性注入"

---

### 3. UserService.cs

**修改前：**
```csharp
public class UserService : BaseService<basic_user>, IUserService
{
    public UserService(ISqlSugarClient db)
    {
        _db = db;
    }
}
```

**修改后：**
```csharp
public class UserService : BaseService<basic_user>, IUserService
{
    // _db 自动注入，无需构造函数
}
```

**变化：**
- ❌ 删除了构造函数
- ✅ `_db` 属性继承自 `BaseService<basic_user>`，会自动注入

---

### 4. BaseService.cs

**修改内容：**
- 更新 XML 注释，明确说明"通过 Autofac 属性注入"
- 添加详细的使用示例

**核心说明：**
```csharp
/// <summary>
/// 数据库上下文（通过 Autofac 属性注入）
/// </summary>
/// <remarks>
/// 使用 SqlSugar 的 ISqlSugarClient 接口，支持多种数据库操作。
/// 通过 Autofac 的属性注入功能自动赋值，无需构造函数。
/// 属性命名规则：以 "_" 开头的公共属性会自动注入。
/// </remarks>
public ISqlSugarClient _db { get; set; } = null!;
```

---

## 📝 使用规则

### ✅ 属性注入命名规则

1. **必须以 `_` 开头**（如 `_userService`, `_db`, `_logger`）
2. **必须是 `public` 属性**
3. **必须有 `set` 方法**
4. **使用 `= null!` 非空断言**

### ✅ 正确示例

```csharp
// ✅ Controller 示例
public class UserController : BaseController
{
    public IUserService _userService { get; set; } = null!;
    public ILogger<UserController> _logger { get; set; } = null!;
}

// ✅ Service 示例
public class UserService : BaseService<User>, IUserService
{
    public ILogger<UserService> _logger { get; set; } = null!;
    // _db 继承自 BaseService<User>，无需重复声明
}

// ✅ 多个依赖示例
public class OrderController : BaseController
{
    public IOrderService _orderService { get; set; } = null!;
    public IUserService _userService { get; set; } = null!;
    public ILogger<OrderController> _logger { get; set; } = null!;
}
```

### ❌ 错误示例

```csharp
// ❌ 错误：私有字段不会被注入
private readonly IUserService _userService;

// ❌ 错误：不以 _ 开头的属性不会被注入
public IUserService UserService { get; set; } = null!;

// ❌ 错误：只读属性无法注入
public IUserService _userService { get; } = null!;

// ❌ 错误：没有 set 方法
public IUserService _userService { get; private set; } = null!;
```

---

## 🎯 优势对比

### 传统构造函数注入

```csharp
public class UserController : BaseController
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;
    private readonly IConfiguration _configuration;

    public UserController(
        IUserService userService,
        ILogger<UserController> logger,
        IConfiguration configuration)
    {
        _userService = userService;
        _logger = logger;
        _configuration = configuration;
    }
}
```

**缺点：**
- 构造函数冗长
- 添加依赖时需要修改构造函数
- 子类需要重复构造函数

### Autofac 属性注入

```csharp
public class UserController : BaseController
{
    public IUserService _userService { get; set; } = null!;
    public ILogger<UserController> _logger { get; set; } = null!;
    public IConfiguration _configuration { get; set; } = null!;
}
```

**优点：**
- ✅ 代码简洁
- ✅ 添加依赖只需添加属性
- ✅ 子类无需重复构造函数
- ✅ BaseService 的 `_db` 自动注入

---

## ⚠️ 注意事项

### 1. 属性注入的执行时机

属性注入发生在对象创建**之后**（`OnActivated` 阶段），因此：
- ✅ 可以在方法中直接使用注入的属性
- ❌ 不能在构造函数中使用（如果有构造函数的话）

### 2. 可选依赖

如果某个依赖可能不存在，可以这样处理：

```csharp
public IOptionalService? _optionalService { get; set; }

// 使用时检查
public void SomeMethod()
{
    if (_optionalService != null)
    {
        // 使用 _optionalService
    }
}
```

### 3. 单元测试

在单元测试中，需要手动设置属性值：

```csharp
[Fact]
public async Task Test_GetUserById()
{
    // Arrange
    var mockDb = new Mock<ISqlSugarClient>();
    var service = new UserService
    {
        _db = mockDb.Object  // ✅ 手动设置属性
    };

    // Act
    var result = await service.GetByIdAsync(Guid.NewGuid());

    // Assert
    // ...
}
```

---

## 🚀 后续工作

### 需要修改的其他 Controller 和 Service

按照相同的模式修改所有 Controller 和 Service：

1. **删除构造函数**
2. **将 `private readonly` 字段改为 `public` 属性**
3. **添加 `= null!` 非空断言**
4. **更新 XML 注释**

### 示例：批量修改

使用全局查找替换：
- 查找：`private readonly I(\w+)Service _(\w+);`
- 替换：`public I$1Service _$2 { get; set; } = null!;`

---

## 📚 参考资料

- [Autofac 官方文档 - Property Injection](https://docs.autofac.org/en/latest/advanced/property-injection.html)
- [EasyWechatWeb 参考实现](D:\4-MyProject\EasyProject\EasyWechatWeb)

---

生成时间: 2026-09-07