using AigioL.Common.AspNetCore.AppCenter.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using KeyValuePair = global::AigioL.Common.AspNetCore.AppCenter.Entities.KeyValuePair;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

partial class AppDbContext : IKeyValuePairsDbContext
{
    public DbSet<KeyValuePair> KeyValuePairs { get; set; } = null!;
}