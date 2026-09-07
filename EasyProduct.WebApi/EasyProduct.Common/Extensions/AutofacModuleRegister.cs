using Autofac;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace EasyProduct.Common.Extensions;

/// <summary>
/// Autofac 模块注册器
/// </summary>
/// <remarks>
/// 提供程序集扫描注册功能，自动注册 Service、Controller、Filter 等。
/// </remarks>
public class AutofacModuleRegister : Autofac.Module
{
    private readonly IConfiguration _configuration;
    private readonly List<string> _assemblies = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="configuration">配置对象</param>
    public AutofacModuleRegister(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 添加程序集
    /// </summary>
    /// <param name="assemblyName">程序集名称</param>
    public void AddAssembly(string assemblyName)
    {
        _assemblies.Add(assemblyName);
    }

    /// <summary>
    /// 添加程序集（通过类型）
    /// </summary>
    /// <param name="type">类型</param>
    public void AddAssembly(Type type)
    {
        _assemblies.Add(type.Assembly.GetName().Name ?? throw new ArgumentException("无法获取程序集名称"));
    }

    /// <summary>
    /// 加载模块
    /// </summary>
    protected override void Load(ContainerBuilder builder)
    {
        // 加载所有指定的程序集
        var assemblies = new List<Assembly>();
        foreach (var assemblyName in _assemblies)
        {
            try
            {
                var assembly = Assembly.Load(assemblyName);
                assemblies.Add(assembly);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"警告：无法加载程序集 {assemblyName}：{ex.Message}");
            }
        }

        if (assemblies.Count == 0)
        {
            throw new InvalidOperationException("没有成功加载任何程序集");
        }

        // 注册服务（接口以 I 开头，实现以 Service 结尾）
        builder.RegisterAssemblyTypes(assemblies.ToArray())
            .Where(t => t.Name.EndsWith("Service") && !t.IsAbstract && !t.IsInterface)
            .AsImplementedInterfaces()
            .AsSelf()
            .InstancePerLifetimeScope()
            .OnActivated(e =>
            {
                InjectProperties(e.Instance, e.Context);
            });

        // 注册控制器
        builder.RegisterAssemblyTypes(assemblies.ToArray())
            .Where(t => t.Name.EndsWith("Controller") && !t.IsAbstract)
            .InstancePerLifetimeScope()
            .OnActivated(e =>
            {
                InjectProperties(e.Instance, e.Context);
            });

        // 注册过滤器
        builder.RegisterAssemblyTypes(assemblies.ToArray())
            .Where(t => t.Name.EndsWith("Filter") && !t.IsAbstract && !t.IsInterface)
            .AsImplementedInterfaces()
            .AsSelf()
            .InstancePerLifetimeScope()
            .OnActivated(e =>
            {
                InjectProperties(e.Instance, e.Context);
            });

        // 注册中间件
        builder.RegisterAssemblyTypes(assemblies.ToArray())
            .Where(t => t.Name.EndsWith("Middleware") && !t.IsAbstract && !t.IsInterface)
            .InstancePerLifetimeScope()
            .OnActivated(e =>
            {
                InjectProperties(e.Instance, e.Context);
            });

        // 注册 Configuration
        builder.RegisterInstance(_configuration).As<IConfiguration>();
    }

    /// <summary>
    /// 手动注入属性（自动注入以 _ 开头的公共属性）
    /// </summary>
    /// <param name="instance">要注入属性的对象实例</param>
    /// <param name="context">Autofac 组件上下文</param>
    /// <remarks>
    /// 注入规则：
    /// 1. 属性必须以 "_" 开头（如 _userService, _db）
    /// 2. 属性必须是 public 且有 set 方法
    /// 3. 使用 = null! 非空断言
    /// </remarks>
    private void InjectProperties(object instance, IComponentContext context)
    {
        var type = instance.GetType();
        var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(p => p.Name.StartsWith("_") && p.CanWrite);

        foreach (var property in properties)
        {
            // 如果属性已有值，跳过注入
            if (property.GetValue(instance) != null)
                continue;

            try
            {
                var serviceType = property.PropertyType;
                if (context.TryResolve(serviceType, out var service))
                {
                    property.SetValue(instance, service);
                }
            }
            catch
            {
                // 忽略解析失败
            }
        }
    }
}
