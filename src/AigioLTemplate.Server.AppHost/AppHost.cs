using System.Reflection;
using static AigioLTemplate.Server.AppHost.AppHostHelper;

var thisType = typeof(Program);
var builder = DistributedApplication.CreateBuilder(args);

// Enables Docker publisher
//builder.AddDockerComposeEnvironment("aspire-docker-aigioltemplate");

#region 数据库

// https://learn.microsoft.com/zh-cn/dotnet/aspire/database/postgresql-integration?tabs=dotnet-cli

#if USE_LOCAL_DB
        var postgres = builder.AddPostgres("postgres", port: postgres_port);
        WithDataBindMount(postgres);
        db_aigioltemplate = postgres.AddDatabase("aigioltemplate");
        db_aigioltemplate_apig = postgres.AddDatabase("aigioltemplate-ag");
#else
db_aigioltemplate = builder.AddParameter("aigioltemplate", $"{DevDbConnectionString};Database=aigioltemplate", secret: true);
db_aigioltemplate_apig = builder.AddParameter("aigioltemplate-ag", $"{DevDbConnectionString};Database=aigioltemplate-ag", secret: true);
#endif

var cache = builder.AddRedis("cache", redis_port);

#endregion

#region 搜索引擎

var conn_meilisearch = thisType.GetField("ConnectionStrings__meilisearch", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null)?.ToString();
if (!string.IsNullOrWhiteSpace(conn_meilisearch))
{
    // 如果部分类定义了常量值，取值添加参数并作为环境变量值
    meilisearch_p = builder.AddParameter("meilisearch", conn_meilisearch, secret: true);
}
else
{
    // https://learn.microsoft.com/zh-cn/dotnet/aspire/community-toolkit/hosting-meilisearch?tabs=dotnet-cli#add-meilisearch-resource-with-data-bind-mount
    var meilisearchPath = GetMeilisearchPath();
    var masterkey = builder.AddParameter("masterkey", secret: true);
    meilisearch = builder.AddMeilisearch("meilisearch", masterkey, meilisearch_port)
                             .WithDataBindMount(
                                 source: meilisearchPath);
}

#endregion

#region 基础 API 服务

var api_bm = builder.AddProject<Projects.AigioLTemplate_Server_ApiService_AdminCenter>("aigioltemplate-api-bm")
    .WithReference(cache)
    .WaitFor(cache)
    .WithPostgresDatabase(db_aigioltemplate)
    .WithHttpHealthCheck("/health");

var api_ms_ba3 = builder.AddProject<Projects.AigioLTemplate_Server_ApiService_Basic>("aigioltemplate-api-ms-ba3")
    .WithReference(cache)
    .WaitFor(cache)
    .WithPostgresDatabase(db_aigioltemplate)
    .WithHttpHealthCheck("/health");

var api_job_ba3 = builder.AddProject<Projects.AigioLTemplate_Server_ApiService_Basic_JobScheduler>("aigioltemplate-api-job-ba3")
    .WithReference(cache)
    .WaitFor(cache)
    .WithPostgresDatabase(db_aigioltemplate)
    .WithHttpHealthCheck("/health");

var api_ms_an7 = builder.AddProject<Projects.AigioLTemplate_Server_ApiService_Analytics>("aigioltemplate-api-ms-an7")
    .WithReference(cache)
    .WaitFor(cache)
    .WithPostgresDatabase(db_aigioltemplate)
    .WithHttpHealthCheck("/health");

var api_job_an7 = builder.AddProject<Projects.AigioLTemplate_Server_ApiService_Analytics_JobScheduler>("aigioltemplate-api-job-an7")
    .WithReference(cache)
    .WaitFor(cache)
    .WithPostgresDatabase(db_aigioltemplate)
    .WithHttpHealthCheck("/health");

var api_ms_id6 = builder.AddProject<Projects.AigioLTemplate_Server_ApiService_Identity>("aigioltemplate-api-ms-id6")
    .WithReference(cache)
    .WaitFor(cache)
    .WithPostgresDatabase(db_aigioltemplate)
    .WithHttpHealthCheck("/health");

var api_ms_or6 = builder.AddProject<Projects.AigioLTemplate_Server_ApiService_Ordering>("aigioltemplate-api-ms-or6")
    .WithReference(cache)
    .WaitFor(cache)
    .WithPostgresDatabase(db_aigioltemplate)
    .WithHttpHealthCheck("/health");

var api_job_or6 = builder.AddProject<Projects.AigioLTemplate_Server_ApiService_Ordering_JobScheduler>("aigioltemplate-api-job-or6")
    .WithReference(cache)
    .WaitFor(cache)
    .WithPostgresDatabase(db_aigioltemplate)
    .WithHttpHealthCheck("/health");

var api_ms_pa5 = builder.AddProject<Projects.AigioLTemplate_Server_ApiService_Payment>("aigioltemplate-api-ms-pa5")
    .WithReference(cache)
    .WaitFor(cache)
    .WithPostgresDatabase(db_aigioltemplate)
    .WithHttpHealthCheck("/health");

var api_job_pa5 = builder.AddProject<Projects.AigioLTemplate_Server_ApiService_Payment_JobScheduler>("aigioltemplate-api-job-pa5")
    .WithReference(cache)
    .WaitFor(cache)
    .WithPostgresDatabase(db_aigioltemplate)
    .WithHttpHealthCheck("/health");

#endregion

#region 业务 API 服务

#endregion

builder.Build().Run();
