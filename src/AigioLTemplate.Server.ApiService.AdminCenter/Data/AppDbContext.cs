using AigioL.Common.AspNetCore.AdminCenter.Entities;
using AigioL.Common.AspNetCore.AppCenter.Data.Abstractions;
using AigioL.Common.AspNetCore.Helpers.ProgramMain;
using AigioL.Common.Repositories.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Design;
using System.Diagnostics.CodeAnalysis;
using AigioL.Common.EntityFrameworkCore.Helpers;


#if PROJ_DBCONTEXT_BM
using AigioL.Common.AspNetCore.AdminCenter.Data.Abstractions;
#endif

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

public sealed partial class AppDbContext
{
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

#if PROJ_DBCONTEXT_BM
        // 由于 BMDbContextBase 不继承自 ACUser 的 IdentityDbContext，故需要调用基类的 OnModelCreatingVersion2 方法
        IAppDbContextBase.OnModelCreatingVersion2(this, b);
#endif

        // 重命名 Identity 相关表名
        IAppDbContextBase.ToIdentitysTable(b);
    }
}

#if PROJ_DBCONTEXT_BM
partial class AppDbContext(
    DbContextOptions<AppDbContext> options) :
    BMDbContextBase<BMUser, BMRole, BMUserRole>(options);
#else
partial class AppDbContext(
    DbContextOptions<AppDbContext> options) :
    AppDbContextBase(options);
#endif

partial class AppDbContext : ProgramHelper.IDbContext
{
    /// <inheritdoc/>
    DbContext ProgramHelper.IDbContext.GetDbContext() => this;
}

partial class AppDbContext : IDbContextBase
{
    /// <inheritdoc/>
    DbContext IDbContextBase.GetDbContext() => this;

    /// <inheritdoc/>
    DatabaseFacade IDbContextBase.GetDatabase() => Database;
}

/// <summary>
/// https://learn.microsoft.com/zh-cn/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#from-a-design-time-factory
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public const bool postgreSQL18Plus = true;

    /// <inheritdoc/>
    public AppDbContext CreateDbContext(string[] args)
    {
        SqlStringHelper.ConfigPostgreSQL(postgreSQL18Plus);

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("");

        return new AppDbContext(optionsBuilder.Options);
    }
}