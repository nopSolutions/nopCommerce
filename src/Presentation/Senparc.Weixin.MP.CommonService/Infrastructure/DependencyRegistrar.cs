using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Weixin;
using Nop.Services.Suppliers;
using Nop.Services.Marketing;

namespace Senparc.Weixin.MP.CommonService.Infrastructure
{
    /// <summary>
    /// Represents a plugin dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<WConfigService>().As<IWConfigService>().InstancePerLifetimeScope();
            builder.RegisterType<WLocationService>().As<IWLocationService>().InstancePerLifetimeScope();
            builder.RegisterType<WMessageBindService>().As<IWMessageBindService>().InstancePerLifetimeScope();
            builder.RegisterType<WMessageService>().As<IWMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<WQrCodeLimitService>().As<IWQrCodeLimitService>().InstancePerLifetimeScope();
            builder.RegisterType<WQrCodeLimitUserService>().As<IWQrCodeLimitUserService>().InstancePerLifetimeScope();
            builder.RegisterType<WUserService>().As<IWUserService>().InstancePerLifetimeScope();
            builder.RegisterType<WUserTagService>().As<IWUserTagService>().InstancePerLifetimeScope();
            builder.RegisterType<QrCodeLimitBindingSourceService>().As<IQrCodeLimitBindingSourceService>().InstancePerLifetimeScope();

            //builder.RegisterType<WMenuButtonService>().As<IWMenuButtonService>().InstancePerLifetimeScope();
            //builder.RegisterType<WAutoreplyNewsInfoService>().As<IWAutoreplyNewsInfoService>().InstancePerLifetimeScope();
            //builder.RegisterType<WMessageAutoReplyService>().As<IWMessageAutoReplyService>().InstancePerLifetimeScope();
            //builder.RegisterType<WKeywordAutoreplyService>().As<IWKeywordAutoreplyService>().InstancePerLifetimeScope();
            //builder.RegisterType<WKeywordAutoreplyKeywordService>().As<IWKeywordAutoreplyKeywordService>().InstancePerLifetimeScope();
            //builder.RegisterType<WKeywordAutoreplyReplyService>().As<IWKeywordAutoreplyReplyService>().InstancePerLifetimeScope();

            //Marketing
            //builder.RegisterType<ActivitiesThemeService>().As<IActivitiesThemeService>().InstancePerLifetimeScope();
            //builder.RegisterType<CustomTeamOrderService>().As<ICustomTeamOrderService>().InstancePerLifetimeScope();
            //builder.RegisterType<CustomTeamService>().As<ICustomTeamService>().InstancePerLifetimeScope();
            //builder.RegisterType<DivisionsCodeChinaService>().As<IDivisionsCodeChinaService>().InstancePerLifetimeScope();
            //builder.RegisterType<MarketingAdvertAddressService>().As<IMarketingAdvertAddressService>().InstancePerLifetimeScope();
            //builder.RegisterType<MarketingAdvertWayService>().As<IMarketingAdvertWayService>().InstancePerLifetimeScope();
            //builder.RegisterType<OfficialCustomerService>().As<IOfficialCustomerService>().InstancePerLifetimeScope();
            //builder.RegisterType<PartnerApplicationFormService>().As<IPartnerApplicationFormService>().InstancePerLifetimeScope();
            //builder.RegisterType<PartnerServiceInfoService>().As<IPartnerServiceInfoService>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductActivitiesThemeMappingService>().As<IProductActivitiesThemeMappingService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAdvertImageService>().As<IProductAdvertImageService>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductExtendLabelService>().As<IProductExtendLabelService>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductGiftProductMappingService>().As<IProductGiftProductMappingService>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductMarketLabelService>().As<IProductMarketLabelService>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductProductExtendLabelMappingService>().As<IProductProductExtendLabelMappingService>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductProductMarketLabelMappingService>().As<IProductProductMarketLabelMappingService>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductUserFollowMappingService>().As<IProductUserFollowMappingService>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductVisitorMappingService>().As<IProductVisitorMappingService>().InstancePerLifetimeScope();
            //builder.RegisterType<PromotionCommissionService>().As<IPromotionCommissionService>().InstancePerLifetimeScope();
            //builder.RegisterType<UserAdvertChannelAnalysisService>().As<IUserAdvertChannelAnalysisService>().InstancePerLifetimeScope();
            builder.RegisterType<UserAssetConsumeHistoryService>().As<IUserAssetConsumeHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<UserAssetIncomeHistoryService>().As<IUserAssetIncomeHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<UserAssetService>().As<IUserAssetService>().InstancePerLifetimeScope();

            //Supplier
            //builder.RegisterType<ProductSupplierVoucherCouponMappingService>().As<IProductSupplierVoucherCouponMappingService>().InstancePerLifetimeScope();
            builder.RegisterType<QrCodeSupplierVoucherCouponMappingService>().As<IQrCodeSupplierVoucherCouponMappingService>().InstancePerLifetimeScope();
            //builder.RegisterType<SupplierImageService>().As<ISupplierImageService>().InstancePerLifetimeScope();
            //builder.RegisterType<SupplierProductMappingService>().As<ISupplierProductMappingService>().InstancePerLifetimeScope();
            //builder.RegisterType<SupplierSelfGroupService>().As<ISupplierSelfGroupService>().InstancePerLifetimeScope();
            //builder.RegisterType<SupplierService>().As<ISupplierService>().InstancePerLifetimeScope();
            builder.RegisterType<SupplierShopService>().As<ISupplierShopService>().InstancePerLifetimeScope();
            //builder.RegisterType<SupplierShopTagMappingService>().As<ISupplierShopTagMappingService>().InstancePerLifetimeScope();
            //builder.RegisterType<SupplierShopTagService>().As<ISupplierShopTagService>().InstancePerLifetimeScope();
            //builder.RegisterType<SupplierShopUserFollowMappingService>().As<ISupplierShopUserFollowMappingService>().InstancePerLifetimeScope();
            builder.RegisterType<SupplierUserAuthorityMappingService>().As<ISupplierUserAuthorityMappingService>().InstancePerLifetimeScope();
            //builder.RegisterType<SupplierVoucherCouponAppliedValueService>().As<ISupplierVoucherCouponAppliedValueService>().InstancePerLifetimeScope();
            builder.RegisterType<SupplierVoucherCouponService>().As<ISupplierVoucherCouponService>().InstancePerLifetimeScope();
            //builder.RegisterType<UserSupplierVoucherCouponService>().As<IUserSupplierVoucherCouponService>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}