using AigioL.Common.AspNetCore.AppCenter.Ordering.Data.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Ordering.Entities;
using AigioL.Common.AspNetCore.AppCenter.Ordering.Entities.Membership;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioLTemplate.Server.ApiService.Data;

partial class AppDbContext : IPaymentDbContext
{
    public DbSet<OrderBusinessPaymentConfiguration> OrderBusinessPaymentConfigurations { get; set; } = null!;

    public DbSet<CooperatorAccount> CooperatorAccounts { get; set; } = null!;

    public DbSet<MerchantDeductionAgreement> MerchantDeductionAgreements { get; set; } = null!;

    public DbSet<MerchantDeductionAgreementConfiguration> MerchantDeductionAgreementConfigurations { get; set; } = null!;

    public DbSet<MembershipBusinessOrder> MembershipBusinessOrders { get; set; } = null!;

    public DbSet<MembershipGoods> MembershipGoods { get; set; } = null!;

    public DbSet<MembershipGoodsMDARelation> MembershipGoodsMDARelations { get; set; } = null!;

    public DbSet<MembershipProductKeyRecord> MembershipProductKeyRecords { get; set; } = null!;

    public DbSet<MembershipGoodsUserFirstRecord> MembershipGoodsUserFirstRecords { get; set; } = null!;
}