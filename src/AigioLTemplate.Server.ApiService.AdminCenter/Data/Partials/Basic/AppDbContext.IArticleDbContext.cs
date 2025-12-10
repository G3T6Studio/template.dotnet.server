using AigioL.Common.AspNetCore.AppCenter.Basic.Data.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Basic.Entities.Articles;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

partial class AppDbContext : IArticleDbContext
{
    public DbSet<Article> Articles { get; set; } = null!;

    public DbSet<ArticleCategory> ArticleCategories { get; set; } = null!;

    public DbSet<ArticleTag> ArticleTags { get; set; } = null!;

    public DbSet<ArticleTagRelation> ArticleTagRelations { get; set; } = null!;

    public DbSet<ArticleVisitStatistic> ArticleVisitStatistics { get; set; } = null!;
}