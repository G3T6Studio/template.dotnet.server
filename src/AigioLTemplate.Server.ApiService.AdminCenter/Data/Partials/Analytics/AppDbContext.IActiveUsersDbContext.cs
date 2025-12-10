using AigioL.Common.AspNetCore.AppCenter.Analytics.Data.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Analytics.Entities.AnalysisLogs;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

partial class AppDbContext : IAnalysisLogDbContext
{
    public DbSet<AnalysisPropertie> AnalysisProperties { get; set; } = null!;

    public DbSet<AnalysisApp> AnalysisApps { get; set; } = null!;

    public DbSet<AnalysisInstall> AnalysisInstalls { get; set; } = null!;

    public DbSet<AnalysisEventLog> AnalysisEventLogs { get; set; } = null!;

    public DbSet<AnalysisStartServiceLog> AnalysisStartServiceLogs { get; set; } = null!;

    public DbSet<AnalysisStartSessionLog> AnalysisStartSessionLogs { get; set; } = null!;

    public DbSet<AnalysisDevice> AnalysisDevices { get; set; } = null!;

    public DbSet<AnalysisService> AnalysisServices { get; set; } = null!;

    public DbSet<AnalysisLogPropertiesRelation> AnalysisLogPropertiesRelations { get; set; } = null!;

    public DbSet<AnalysisServiceLogRelation> AnalysisServiceLogRelations { get; set; } = null!;
}