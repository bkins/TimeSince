using Plugin.InAppBilling;

namespace TimeSince.Services.ServicesIntegration;

public partial class AppIntegrationService // InAppPurchasing Service Methods
{
    public async void MakePurchase()
    {
        await _inAppPurchasing.MakePurchase(productId: ""
                                          , ItemType.InAppPurchase
                                          , obfuscatedAccountId: ""
                                          , obfuscatedProfileId: ""
                                          , subOfferToken: "");
    }

    public void PurchaseItem()
    {
       // _inAppPurchasing.PurchaseItem()
    }
}
