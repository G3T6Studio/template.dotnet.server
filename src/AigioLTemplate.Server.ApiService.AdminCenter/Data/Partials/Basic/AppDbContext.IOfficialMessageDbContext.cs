using AigioL.Common.AspNetCore.AppCenter.Basic.Data.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Basic.Entities.OfficialMessages;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

partial class AppDbContext : IOfficialMessageDbContext
{
    public DbSet<OfficialMessage> OfficialMessages { get; set; } = null!;

    public DbSet<OfficialMessageAppVerRelation> OfficialMessageAppVerRelations { get; set; } = null!;
}