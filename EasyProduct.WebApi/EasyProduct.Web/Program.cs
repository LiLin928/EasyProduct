using Autofac;
using Autofac.Extensions.DependencyInjection;
using EasyProduct.Common.Extensions;
using EasyProduct.Common.Logging;
using EasyProduct.Web.Middleware;
using Serilog;

// ============================================
// 1. Serilog 初始化（最早执行）
// ============================================
SerilogConfiguration.Configure();

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 2. 配置加载
// ============================================
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// ============================================
// 3. Serilog 集成
// ============================================
builder.Host.UseSerilog();

// ============================================
// 4. Autofac DI 容器替换
// ============================================
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // 创建 Autofac 模块注册器
    var module = new AutofacModuleRegister(builder.Configuration);
    
    // 添加各层程序集
    module.AddAssembly("EasyProduct.Business");
    module.AddAssembly("EasyProduct.Common");
    module.AddAssembly(typeof(Program).Assembly.GetName().Name ?? "EasyProduct.Web");
    
    containerBuilder.RegisterModule(module);
});

// ============================================
// 5. 服务注册
// ============================================

// SqlSugar
builder.Services.AddSqlSugarService(builder.Configuration);

// HttpClient
builder.Services.AddHttpClient();

// HttpContextAccessor（用于获取客户端 IP、User-Agent 等）
builder.Services.AddHttpContextAccessor();

// 注册 JwtHelper（用于生成 JWT Token）
builder.Services.AddScoped<EasyProduct.Common.Helper.JwtHelper>();

// Swagger（开发环境）
if (builder.Configuration.GetValue<bool>("IsUseSwagger"))
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "EasyProduct API",
            Version = "v1",
            Description = "EasyProduct 模块化单体 API"
        });
    });
}

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT 认证（可选，根据配置启用）
var jwtOptions = builder.Configuration.GetSection("JWTTokenOptions");
if (!string.IsNullOrEmpty(jwtOptions.GetValue<string>("SecretKey")))
{
    builder.Services.AddAuthentication()
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.GetValue<string>("Issuer"),
                ValidAudience = jwtOptions.GetValue<string>("Audience"),
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(jwtOptions.GetValue<string>("SecretKey")!))
            };
        });
}

// 限流策略（会员端 API）
builder.Services.AddRateLimiter(options =>
{
    // 添加滑动窗口限流策略
    options.AddPolicy("AppPolicy", context =>
        System.Threading.RateLimiting.RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new System.Threading.RateLimiting.SlidingWindowRateLimiterOptions
            {
                PermitLimit = 100,  // 每分钟最多 100 次请求
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 4
            }));

    // 限流拒绝响应
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            code = 429,
            message = "请求过于频繁，请稍后再试",
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        }, cancellationToken);
    };
});

// ============================================
// 6. 构建应用
// ============================================
var app = builder.Build();

// ============================================
// 7. 中间件管道配置
// ============================================

// 异常处理（最外层）
app.UseExceptionHandling();

// 开发环境 Swagger
if (app.Configuration.GetValue<bool>("IsUseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS 重定向
app.UseHttpsRedirection();

// 静态文件服务
app.UseStaticFiles();

// CORS
app.UseCors("AllowSpecific");

// 限流（在认证之前，拦截恶意请求）
app.UseRateLimiter();

// 认证授权
app.UseAuthentication();
app.UseAuthorization();

// 请求日志
app.UseRequestLogging();

// 控制器路由
app.MapControllers();

// ============================================
// 8. 启动运行
// ============================================
Log.Information("EasyProduct API 启动成功");
app.Run();
