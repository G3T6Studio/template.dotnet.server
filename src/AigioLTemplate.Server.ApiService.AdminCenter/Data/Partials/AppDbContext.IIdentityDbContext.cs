using AigioL.Common.AspNetCore.AppCenter.Data.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Entities;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

partial class AppDbContext : IIdentityDbContext { }

#if PROJ_DBCONTEXT_BM
partial class AppDbContext
{
    #region 用户模块

    DbSet<User> IIdentityDbContext.Users => ACUsers;

    public DbSet<UserDelete> ACUserDeletes { get; set; } = null!;

    DbSet<UserDelete> IIdentityDbContext.UserDeletes => ACUserDeletes;

    public DbSet<UserDevice> ACUserDevices { get; set; } = null!;

    DbSet<UserDevice> IIdentityDbContext.UserDevices => ACUserDevices;

    public DbSet<UserWallet> ACUserWallets { get; set; } = null!;

    DbSet<UserWallet> IIdentityDbContext.UserWallets => ACUserWallets;

    public DbSet<UserWalletChangeRecord> ACUserWalletChangeRecords { get; set; } = null!;

    DbSet<UserWalletChangeRecord> IIdentityDbContext.UserWalletChangeRecords => ACUserWalletChangeRecords;

    public DbSet<ExternalAccount> ACExternalAccounts { get; set; } = null!;

    DbSet<ExternalAccount> IIdentityDbContext.ExternalAccounts => ACExternalAccounts;

    public DbSet<UserDeleteExternalAccount> ACUserDeleteExternalAccounts { get; set; } = null!;

    DbSet<UserDeleteExternalAccount> IIdentityDbContext.UserDeleteExternalAccounts => ACUserDeleteExternalAccounts;

    public DbSet<UserMembership> ACUserMemberships { get; set; } = null!;

    DbSet<UserMembership> IIdentityDbContext.UserMemberships => ACUserMemberships;

    public DbSet<UserMembershipChangeRecord> ACUserMembershipChangeRecords { get; set; } = null!;

    DbSet<UserMembershipChangeRecord> IIdentityDbContext.UserMembershipChangeRecords => ACUserMembershipChangeRecords;

    #endregion

    #region JsonWebToken

    public DbSet<UserJsonWebToken> ACUserJsonWebTokens { get; set; } = null!;

    DbSet<UserJsonWebToken> IIdentityDbContext.UserJsonWebTokens => ACUserJsonWebTokens;

    public DbSet<UserRefreshJsonWebToken> ACUserRefreshJsonWebTokens { get; set; } = null!;

    DbSet<UserRefreshJsonWebToken> IIdentityDbContext.UserRefreshJsonWebTokens => ACUserRefreshJsonWebTokens;

    #endregion
}
#endif