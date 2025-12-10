using AigioL.Common.AspNetCore.AppCenter.Data.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Entities;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

partial class AppDbContext : IAppDbContextBase
{
#if PROJ_DBCONTEXT_BM
    public DbSet<User> ACUsers { get; set; } = null!;

    DbSet<User> IAppDbContextBase.Users => ACUsers;

    public DbSet<UserClaim> ACUserClaims { get; set; } = null!;

    DbSet<UserClaim> IAppDbContextBase.UserClaims => ACUserClaims;

    public DbSet<UserLogin> ACUserLogins { get; set; } = null!;

    DbSet<UserLogin> IAppDbContextBase.UserLogins => ACUserLogins;

    public DbSet<UserToken> ACUserTokens { get; set; } = null!;

    DbSet<UserToken> IAppDbContextBase.UserTokens => ACUserTokens;

    public DbSet<UserRole> ACUserRoles { get; set; } = null!;

    DbSet<UserRole> IAppDbContextBase.UserRoles => ACUserRoles;

    public DbSet<Role> ACRoles { get; set; } = null!;

    DbSet<Role> IAppDbContextBase.Roles => ACRoles;

    public DbSet<RoleClaim> ACRoleClaims { get; set; } = null!;

    DbSet<RoleClaim> IAppDbContextBase.RoleClaims => ACRoleClaims;
#endif
}