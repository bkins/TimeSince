using Plugin.InAppBilling;

namespace TimeSince.Services.ServicesIntegration;

/// The AppIntegrationService class handles the integration with an in-app purchasing service. It provides methods for making a purchase and purchasing an item.
/// /
public partial class AppIntegrationService // InAppPurchasing Service Methods
{
    public async Task<bool> MakePurchase()
    {
        if( ! HasInternetAccess) return false;

        return await _inAppPurchasing.MakePurchase(productId: "1234");
    }

    public void PurchaseItem()
    {
       // _inAppPurchasing.PurchaseItem()
    }
}
