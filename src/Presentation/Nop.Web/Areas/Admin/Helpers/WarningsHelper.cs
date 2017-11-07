using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Security;

namespace Nop.Web.Areas.Admin.Helpers
{
    /// <summary>
    /// Warnings helper
    /// </summary>
    public static class WarningsHelper
    {
        public static List<SystemWarningModel> GetWarnings()
        {
            var catalogSettings = EngineContext.Current.Resolve<CatalogSettings>();
            var currencySettings = EngineContext.Current.Resolve<CurrencySettings>();
            var measureSettings = EngineContext.Current.Resolve<MeasureSettings>();
            var currencyService = EngineContext.Current.Resolve<ICurrencyService>();
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var measureService = EngineContext.Current.Resolve<IMeasureService>();
            var paymentService = EngineContext.Current.Resolve<IPaymentService>();
            var storeContext = EngineContext.Current.Resolve<IStoreContext>();
            var storeService = EngineContext.Current.Resolve<IStoreService>();
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();

            var model = new List<SystemWarningModel>();

            //store URL
            var currentStoreUrl = storeContext.CurrentStore.Url;
            if (!string.IsNullOrEmpty(currentStoreUrl) &&
                (currentStoreUrl.Equals(webHelper.GetStoreLocation(false), StringComparison.InvariantCultureIgnoreCase)
                ||
                currentStoreUrl.Equals(webHelper.GetStoreLocation(true), StringComparison.InvariantCultureIgnoreCase)
                ))
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = localizationService.GetResource("Admin.System.Warnings.URL.Match")
                });
            else
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = string.Format(localizationService.GetResource("Admin.System.Warnings.URL.NoMatch"), currentStoreUrl, webHelper.GetStoreLocation(false))
                });

            //primary exchange rate currency
            var perCurrency = currencyService.GetCurrencyById(currencySettings.PrimaryExchangeRateCurrencyId);
            if (perCurrency != null)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = localizationService.GetResource("Admin.System.Warnings.ExchangeCurrency.Set"),
                });
                if (perCurrency.Rate != 1)
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Fail,
                        Text = localizationService.GetResource("Admin.System.Warnings.ExchangeCurrency.Rate1")
                    });
                }
            }
            else
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = localizationService.GetResource("Admin.System.Warnings.ExchangeCurrency.NotSet")
                });
            }

            //primary store currency
            var pscCurrency = currencyService.GetCurrencyById(currencySettings.PrimaryStoreCurrencyId);
            if (pscCurrency != null)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = localizationService.GetResource("Admin.System.Warnings.PrimaryCurrency.Set"),
                });
            }
            else
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = localizationService.GetResource("Admin.System.Warnings.PrimaryCurrency.NotSet")
                });
            }

            //base measure weight
            var bWeight = measureService.GetMeasureWeightById(measureSettings.BaseWeightId);
            if (bWeight != null)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = localizationService.GetResource("Admin.System.Warnings.DefaultWeight.Set"),
                });

                if (bWeight.Ratio != 1)
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Fail,
                        Text = localizationService.GetResource("Admin.System.Warnings.DefaultWeight.Ratio1")
                    });
                }
            }
            else
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = localizationService.GetResource("Admin.System.Warnings.DefaultWeight.NotSet")
                });
            }

            //base dimension weight
            var bDimension = measureService.GetMeasureDimensionById(measureSettings.BaseDimensionId);
            if (bDimension != null)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = localizationService.GetResource("Admin.System.Warnings.DefaultDimension.Set"),
                });

                if (bDimension.Ratio != 1)
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Fail,
                        Text = localizationService.GetResource("Admin.System.Warnings.DefaultDimension.Ratio1")
                    });
                }
            }
            else
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = localizationService.GetResource("Admin.System.Warnings.DefaultDimension.NotSet")
                });
            }

            //payment methods
            if (paymentService.LoadActivePaymentMethods().Any())
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = localizationService.GetResource("Admin.System.Warnings.PaymentMethods.OK")
                });
            else
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Fail,
                    Text = localizationService.GetResource("Admin.System.Warnings.PaymentMethods.NoActive")
                });

            //incompatible plugins
            if (PluginManager.IncompatiblePlugins != null)
                foreach (var pluginName in PluginManager.IncompatiblePlugins)
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = string.Format(localizationService.GetResource("Admin.System.Warnings.PluginNotLoaded"), pluginName)
                    });

            //performance settings
            if (!catalogSettings.IgnoreStoreLimitations && storeService.GetAllStores().Count == 1)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = localizationService.GetResource("Admin.System.Warnings.Performance.IgnoreStoreLimitations")
                });
            }
            if (!catalogSettings.IgnoreAcl)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = localizationService.GetResource("Admin.System.Warnings.Performance.IgnoreAcl")
                });
            }

            //validate write permissions (the same procedure like during installation)
            var dirPermissionsOk = true;
            var dirsToCheck = FilePermissionHelper.GetDirectoriesWrite();
            foreach (var dir in dirsToCheck)
                if (!FilePermissionHelper.CheckPermissions(dir, false, true, true, false))
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = string.Format(localizationService.GetResource("Admin.System.Warnings.DirectoryPermission.Wrong"), WindowsIdentity.GetCurrent().Name, dir)
                    });
                    dirPermissionsOk = false;
                }
            if (dirPermissionsOk)
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = localizationService.GetResource("Admin.System.Warnings.DirectoryPermission.OK")
                });

            var filePermissionsOk = true;
            var filesToCheck = FilePermissionHelper.GetFilesWrite();
            foreach (var file in filesToCheck)
                if (!FilePermissionHelper.CheckPermissions(file, false, true, true, true))
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = string.Format(localizationService.GetResource("Admin.System.Warnings.FilePermission.Wrong"), WindowsIdentity.GetCurrent().Name, file)
                    });
                    filePermissionsOk = false;
                }
            if (filePermissionsOk)
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = localizationService.GetResource("Admin.System.Warnings.FilePermission.OK")
                });

            return model;
        }

    }
}