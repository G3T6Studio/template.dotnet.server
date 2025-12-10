using AigioL.Common.AspNetCore.AdminCenter;
using AigioL.Common.AspNetCore.AdminCenter.Entities;
using AigioL.Common.AspNetCore.AdminCenter.Models;
using AigioL.Common.AspNetCore.AdminCenter.Policies.Handlers;
using AigioL.Common.AspNetCore.Helpers.ProgramMain;
using AigioL.Common.AspNetCore.Helpers.ProgramMain.Controllers.Infrastructure;
using AigioL.Common.JsonWebTokens.Models.Abstractions;
using AigioLTemplate.Server.ApiService.AdminCenter.Models;
using AigioLTemplate.Server.ApiService.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

const bool UseHttps = true;
unsafe
{
    ProgramHelper.M(Path.GetFileNameWithoutExtension(Environment.ProcessPath!), args, &ConfigureServices, &Configure);
}

static void ConfigureServices(WebApplicationBuilder builder)
{
    if (UseHttps)
    {
        builder.WebHost.UseKestrelHttpsConfiguration();
    }

    // Add service defaults & Aspire client integrations.
    builder.AddServiceDefaults();

    // Add services to the container.
    builder.Services.AddBMApiRspProblemDetails();

    // 配置 JSON 源生成序列化上下文
    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.TypeInfoResolverChain.Insert(0, BMMinimalApisJsonSerializerContext.Default);
    });

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi(options =>
    {
        // https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/openapi/customize-openapi#use-document-transformers
        options.AddBMBearerSecuritySchemeTransformer();
    });
    builder.Services.AddValidation();

    // 读取配置 AppSettings
    var appSettingsSection = builder.Configuration.GetRequiredSection("AppSettings");
    builder.Services.Configure<AppSettings>(appSettingsSection);
    builder.Services.TryAddSingleton<IOptions<BMAppSettings>>(static s => s.GetRequiredService<IOptions<AppSettings>>()); // 添加父类的配置项
    var appSettings = appSettingsSection.Get<AppSettings>();
    ArgumentNullException.ThrowIfNull(appSettings);
    var signingKey = IJsonWebTokenOptions.GetSymmetricSecurityKey(appSettings);

    // 配置允许跨域的地址
    builder.Services.AddCorsByViewUrl(appSettings);

    // 配置 JWT
    builder.Services.AddAuthorization();
    builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler<AppDbContext, BMUser, BMRole, BMUserRole>>();
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = BMMinimalApis.BearerScheme;
        options.DefaultAuthenticateScheme = BMMinimalApis.BearerScheme;
        options.DefaultChallengeScheme = BMMinimalApis.BearerScheme;
    })
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = true;
        o.SaveToken = true;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // 签名的密钥必须校验
            ValidateLifetime = true, // 验证凭证的时间是否过期
            ClockSkew = TimeSpan.Zero, // 时间不能有偏差
            IssuerSigningKey = signingKey,
            ValidIssuer = appSettings.Issuer,
            ValidateIssuer = true,
            ValidAudience = appSettings.Audience,
            ValidateAudience = true,
        };
    });

    // 添加 Redis 分布式缓存服务
    builder.AddRedisDistributedCacheV2(connectionName: "cache");

    // 配置数据库上下文
    const string connectionStringACKey = "g3t6";
    var connectionStringAC = ProgramHelper.GetConnectionString(connectionStringACKey,
#if DEBUG
        "00000000-0000-0000-2507-000000000001",
        null,
#endif
        builder);
    builder.Services.AddDbContext2<AppDbContext>(options =>
        options.UseNpgsql(connectionStringAC)
    );
    // https://learn.microsoft.com/zh-cn/dotnet/aspire/database/postgresql-entity-framework-integration?tabs=dotnet-cli#enrich-an-npgsql-database-context
    //builder.EnrichNpgsqlDbContext<ACDbContext>(
    //    configureSettings: settings =>
    //    {
    //        settings.DisableRetry = false;
    //        settings.CommandTimeout = 30;
    //    });
    builder.Services.AddScoped<ProgramHelper.IDbContext>(static s => s.GetRequiredService<AppDbContext>());

    // 配置管理后台身份认证
    builder.Services.AddIdentityCore<BMUser>(options =>
    {
        options.SetBMDefaults();
    }).AddRoles<BMRole>().AddEntityFrameworkStores<AppDbContext>();

    // 添加管理后台仓储层服务
    builder.Services.AddACRepositories<AppDbContext, BMUser, BMRole, BMUserRole>();
}

static void Configure(WebApplication app)
{
    var isDevelopment = app.Environment.IsDevelopment();
    var appSettings = app.Services.GetRequiredService<IOptions<AppSettings>>().Value;

    // 启用修复反向代理导致请求方 IP 地址不正确的问题
    app.UseForwardedHeaders(appSettings);
    app.UseCors(appSettings);

    // Configure the HTTP request pipeline.
    app.UseBMApiRspExceptionHandler();

    if (isDevelopment)
    {
        //app.UseMigrationsEndPoint();
        app.MapOpenApi();
        app.MapScalarApiReference("/", options =>
        {
            options.Title = $"{ProgramHelper.ProjectName} Scalar API Reference";
        });
    }
    else
    {
        app.UseWelcomePage("/");
    }

    // 鉴权，检测有没有登录，登录的是谁，赋值给 User
    app.UseAuthentication();

    // 授权，检测权限
    app.UseAuthorization();

    app.MapDefaultEndpoints();

    // 配置后台管理通用终结点路由
    app.MapBMMinimalApis<BMUser, BMRole>();

    // 配置 api/info GET 终结点路由
    app.MapGetInfo();
    app.MapGetIpV6();
    app.MapGetIpVal();

#if DEBUG
    app.MapGetRSADemoHtml();
    app.MapGet(ProgramHelper.GetEndpointPattern("{projId}/test/ex"), () =>
    {
        throw new Exception("This is an example exception for testing purposes.");
    }).WithDescription("测试异常处理");
    app.MapGet(ProgramHelper.GetEndpointPattern("{projId}/test/ex/db"), () =>
    {
        throw new global::Npgsql.NpgsqlException("This is an example exception for testing purposes.");
    }).WithDescription("测试数据库异常处理");
    app.MapGet(ProgramHelper.GetEndpointPattern("{projId}/test/statusCode/{statusCode}"), ([FromRoute] int statusCode) =>
    {
        return Results.StatusCode(statusCode);
    }).WithDescription("测试状态码响应");
#endif
}
