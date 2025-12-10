using AigioL.Common.AspNetCore.AppCenter.Analytics.Data.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Analytics.Entities.ActiveUsers;
using AigioL.Common.AspNetCore.AppCenter.Analytics.Entities.AnalysisLogs.Summaries;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

partial class AppDbContext : IAnalysisLogSummariesDbContext
{
    public DbSet<AnalysisEventLogSummary> AnalysisEventLogSummaries { get; set; } = null!;

    public DbSet<AnalysisStartServiceLogSummary> AnalysisStartServiceLogSummaries { get; set; } = null!;

    public DbSet<AnalysisStartSessionLogSummary> AnalysisStartSessionLogSummaries { get; set; } = null!;

    public DbSet<EventRelatedPropertieSummary> EventRelatedPropertieSummaries { get; set; } = null!;
}