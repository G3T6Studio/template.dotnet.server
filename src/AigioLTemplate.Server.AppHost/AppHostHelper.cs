namespace AigioLTemplate.Server.AppHost;

static partial class AppHostHelper
{
    internal const string repoName = "AigioLTemplate.Server";
    internal const int postgres_port = 30001;
    internal const int redis_port = 30002;
    internal const int meilisearch_port = 30003;

#if USE_LOCAL_DB
    internal static IResourceBuilder<PostgresDatabaseResource> db_aigioltemplate = null!;
    internal static IResourceBuilder<PostgresDatabaseResource> db_aigioltemplate_apig = null!;
#else
    internal static IResourceBuilder<ParameterResource> db_aigioltemplate = null!;
    internal static IResourceBuilder<ParameterResource> db_aigioltemplate_apig = null!;
#endif

    internal static IResourceBuilder<MeilisearchResource> meilisearch = null!;
    internal static IResourceBuilder<ParameterResource> meilisearch_p = null!;

    internal const string ConnectionStringEnvironmentName = "ConnectionStrings__";

    internal static string? GetPostgreSQLDatabasePath()
    {
        if (OperatingSystem.IsWindows())
        {
            var projPath = ProjPath;
            if (string.IsNullOrWhiteSpace(projPath))
            {
                return $@"C:\PostgreSQL\{repoName}";
            }
            // 此路径已在 .gitignore 中忽略
            return Path.Combine(projPath, "res", "postgresql", "data");
        }
        else
        {
            return null;
        }
    }

    internal static string GetMeilisearchPath()
    {
        if (OperatingSystem.IsWindows())
        {
            var projPath = ProjPath;
            if (string.IsNullOrWhiteSpace(projPath))
            {
                return $@"C:\Meilisearch\{repoName}";
            }
            // 此路径已在 .gitignore 中忽略
            return Path.Combine(projPath, "res", "meilisearch", "data");
        }
        else
        {
            throw new PlatformNotSupportedException(); // 尚未适配其他操作系统
        }
    }

    internal static void WithDataBindMount(IResourceBuilder<PostgresServerResource> builder)
    {
        var databasePath = GetPostgreSQLDatabasePath();
        if (databasePath != null)
        {
            builder.WithDataBindMount(source: databasePath, isReadOnly: false);
        }
    }

    internal static IResourceBuilder<TDestination> WithPostgresDatabase<TDestination>(
        this IResourceBuilder<TDestination> builder,
#if USE_LOCAL_DB
        IResourceBuilder<PostgresDatabaseResource> db
#else
        IResourceBuilder<ParameterResource> db
#endif
        )
        where TDestination : IResourceWithEnvironment, IResourceWithWaitSupport
    {
#if USE_LOCAL_DB
        builder.WithReference(db);
        builder.WaitFor(db);
#else
        builder.WithEnvironment($"{ConnectionStringEnvironmentName}{db.Resource.Name}", db);
#endif
        return builder;
    }

    internal static IResourceBuilder<TDestination> WithMeilisearch<TDestination>(
        this IResourceBuilder<TDestination> builder
    )
        where TDestination : IResourceWithEnvironment
    {
        if (meilisearch_p != null)
        {
            builder.WithEnvironment($"{ConnectionStringEnvironmentName}{meilisearch_p.Resource.Name}", meilisearch_p);
        }
        else if (meilisearch != null)
        {
            builder.WithReference(meilisearch);
        }
        return builder;
    }
}
