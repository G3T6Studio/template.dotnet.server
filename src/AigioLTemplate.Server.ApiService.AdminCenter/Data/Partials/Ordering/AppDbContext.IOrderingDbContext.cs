using AigioL.Common.AspNetCore.AppCenter.Ordering.Data.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Ordering.Entities;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

partial class AppDbContext : IOrderingDbContext
{
    public DbSet<FeeType> FeeTypes { get; set; } = null!;

    public DbSet<Coupon> Coupons { get; set; } = null!;

    public DbSet<UserCouponInfo> UserCouponInfos { get; set; } = null!;

    public DbSet<AftersalesBill> AftersalesBills { get; set; } = null!;

    public DbSet<RefundBill> RefundBills { get; set; } = null!;
}