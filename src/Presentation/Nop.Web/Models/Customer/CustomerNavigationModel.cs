﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Customer;

public partial record CustomerNavigationModel : BaseNopModel
{
    public CustomerNavigationModel()
    {
        CustomerNavigationItems = new List<CustomerNavigationItemModel>();
    }

    public IList<CustomerNavigationItemModel> CustomerNavigationItems { get; set; }

    public int SelectedTab { get; set; }
}

public partial record CustomerNavigationItemModel : BaseNopModel
{
    public string RouteName { get; set; }
    public string Title { get; set; }
    public int Tab { get; set; }
    public string ItemClass { get; set; }
}

public enum CustomerNavigationEnum
{
    Info = 0,
    Addresses = 10,
    Orders = 20,
    BackInStockSubscriptions = 30,
    ReturnRequests = 40,
    DownloadableProducts = 50,
    RewardPoints = 60,
    ChangePassword = 70,
    Avatar = 80,
    ForumSubscriptions = 90,
    ProductReviews = 100,
    VendorInfo = 110,
    GdprTools = 120,
    CheckGiftCardBalance = 130,
    MultiFactorAuthentication = 140,
    RecurringPayments = 150,
}