using AigioL.Common.AspNetCore.AppCenter.Ordering.Data.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Ordering.Entities;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

partial class AppDbContext : IOrderingPaymentBaseDbContext
{
    public DbSet<Order> Orders { get; set; } = null!;

    public DbSet<OrderPaymentComposition> OrderPaymentCompositions { get; set; } = null!;

    public DbSet<TransferOrder> TransferOrders { get; set; } = null!;
}