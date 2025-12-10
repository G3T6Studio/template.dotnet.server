using AigioL.Common.AspNetCore.AppCenter.Analytics.Data.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Analytics.Entities.ActiveUsers.Summaries;
using AigioL.Common.AspNetCore.AppCenter.Analytics.Entities.AnalysisLogs.Summaries;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

partial class AppDbContext : IActiveUsersSummariesDbContext
{
    public DbSet<ActiveUserScreenResolutionSummary> ActiveUserScreenResolutionSummaries { get; set; } = null!;

    public DbSet<ActiveUserStatisticSummary> ActiveUserStatisticSummaries { get; set; } = null!;

    public DbSet<ActiveUserPlatformSummary> ActiveUserPlatformSummaries { get; set; } = null!;

    public DbSet<ActiveUserOSSummary> ActiveUserOSSummaries { get; set; } = null!;

    public DbSet<ActiveUserArchitectureSummary> ActiveUserArchitectureSummaries { get; set; } = null!;

    public DbSet<ActiveUserDayWeekMonthSummary> ActiveUserDayWeekMonthSummaries { get; set; } = null!;

    public DbSet<ActiveUserDeviceSummary> ActiveUserDeviceSummaries { get; set; } = null!;

    public DbSet<ActiveUserLanguageSummary> ActiveUserLanguageSummaries { get; set; } = null!;

    public DbSet<ActiveUserAppVerSummary> ActiveUserAppVerSummaries { get; set; } = null!;
}