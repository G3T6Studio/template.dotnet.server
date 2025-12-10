using AigioL.Common.AspNetCore.AppCenter.Analytics.Data.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Entities.Komaasharus.Summaries;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

partial class AppDbContext : IKomaasharuSummariesDbContext
{
    public DbSet<KomaasharuStatistic> KomaasharuStatistics { get; set; } = null!;

    public DbSet<KomaasharuStatisticPerDaySummary> KomaasharuStatisticPerDaySummaries { get; set; } = null!;
}