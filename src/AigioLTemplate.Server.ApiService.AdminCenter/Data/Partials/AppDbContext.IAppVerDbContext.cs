using AigioL.Common.AspNetCore.AppCenter.Basic.Data.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Basic.Entities.AppVersions;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

partial class AppDbContext : IAppVerDbContext
{
    public DbSet<AppVer> AppVers { get; set; } = null!;

    public DbSet<AppVerBuild> AppVerBuilds { get; set; } = null!;

    public DbSet<AppVerFile> AppVerFiles { get; set; } = null!;
}