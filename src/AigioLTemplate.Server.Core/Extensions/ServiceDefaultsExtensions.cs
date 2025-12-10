using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.Hosting;

// Adds common Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static partial class ServiceDefaultsExtensions
{
    const string HealthEndpointPath = "/health";
    const string AlivenessEndpointPath = "/alive";

    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        // Uncomment the following to restrict the allowed schemes for service discovery.
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });

        return builder;
    }

    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation(tracing =>
                        // Exclude health check requests from tracing
                        tracing.Filter = context =>
                            !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                            && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
                    )
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        return builder;
    }

    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks(HealthEndpointPath);

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }

    /// <summary>
    /// 设置用于客户端微服务的 <see cref="IdentityOptions"/> 预设值
    /// </summary>
    /// <param name="options"></param>
    public static void SetMSDefaults(this IdentityOptions options)
    {
        // 密码设置
        options.Password.RequireDigit = false; // 密码吗必须包含数字
        options.Password.RequireLowercase = false; // 密码必须包含小写字母
        options.Password.RequireNonAlphanumeric = false; // 密码必须包含非字母数字（符号）字符
        options.Password.RequireUppercase = false; // 密码必须包含大写字母
        options.Password.RequiredLength = 8; // 设置密码必须具有的最小长度
        options.Password.RequiredUniqueChars = 3; // 设置密码必须包含的唯一字符的最小数目

        // 锁定设置
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3); // 默认锁定时间间隔
        options.Lockout.MaxFailedAccessAttempts = 6; // 最大失败访问尝试次数
        options.Lockout.AllowedForNewUsers = true; // 是否允许新用户锁定

        // 用户设置
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // 允许的用户名字符
        options.User.RequireUniqueEmail = false; // 是否要求用户必须有邮箱

        // 登录设置
        options.SignIn.RequireConfirmedEmail = false; // 是否要求已确认的电子邮件才能登录
        options.SignIn.RequireConfirmedPhoneNumber = true; // 是否要求已确认的电话号码才能登录

        // Differences from Version 1:
        // - maxKeyLength defaults to 128
        // - PhoneNumber has a 256 max length
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version2;
    }

    /// <summary>
    /// 设置用于管理后台服务的 <see cref="IdentityOptions"/> 预设值
    /// </summary>
    /// <param name="options"></param>
    public static void SetBMDefaults(this IdentityOptions options)
    {
        // 密码设置
        options.Password.RequireDigit = true; // 密码吗必须包含数字
        options.Password.RequireLowercase = true; // 密码必须包含小写字母
        options.Password.RequireNonAlphanumeric = true; // 密码必须包含非字母数字（符号）字符
        options.Password.RequireUppercase = true; // 密码必须包含大写字母
        options.Password.RequiredLength = 10; // 设置密码必须具有的最小长度
        options.Password.RequiredUniqueChars = 3; // 设置密码必须包含的唯一字符的最小数目

        // 锁定设置
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10); // 默认锁定时间间隔
        options.Lockout.MaxFailedAccessAttempts = 6; // 最大失败访问尝试次数
        options.Lockout.AllowedForNewUsers = true; // 是否允许新用户锁定

        // 用户设置
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // 允许的用户名字符
        options.User.RequireUniqueEmail = false; // 是否要求用户必须有邮箱

        // 登录设置
        options.SignIn.RequireConfirmedAccount = true; // 需要已 IUserConfirmation<TUser> 确认的帐户才能登录

        // Differences from Version 1:
        // - maxKeyLength defaults to 128
        // - PhoneNumber has a 256 max length
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version2;
    }
}
