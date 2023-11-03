using System.Diagnostics;
using Plugin.InAppBilling;

namespace TimeSince.Services;

public class InAppPurchasing
{
    public async Task<bool> MakePurchase(string   productId
                                       , ItemType itemType            = ItemType.InAppPurchase
                                       , string   obfuscatedAccountId = null
                                       , string   obfuscatedProfileId = null
                                       , string   subOfferToken       = null)
    {
        if (! CrossInAppBilling.IsSupported)
            return false;

        IInAppBilling billing = null;

        try
        {
            billing = CrossInAppBilling.Current;

            if (billing is null) return false;

            var connected = await billing.ConnectAsync();

            if (! connected) return false;

            //make additional billing calls
            billing?.PurchaseAsync(productId
                                 , itemType
                                 , obfuscatedAccountId
                                 , obfuscatedProfileId
                                 , subOfferToken);

            return true;
        }
        finally
        {
            await billing?.DisconnectAsync()!;
        }
    }

    public async Task<IEnumerable<(string Id, bool Success)>> PurchaseItem(string   productId
                                                                         , ItemType itemType = ItemType.InAppPurchase)
    {
        var billing       = CrossInAppBilling.Current;
        var emptyPurchase = new List<(string Id, bool Success)>();

        try
        {
            var connected = await billing.ConnectAsync();

            if ( ! connected) return emptyPurchase;

            //check purchases
            var purchase = await billing.PurchaseAsync(productId
                                                     , itemType);

            //possibility that a null came through.
            if (purchase == null
             || purchase.State != PurchaseState.Purchased)
            {
                //did not purchase
                Debug.WriteLine("purchase is null or purchase state is not Purchased.");

                return emptyPurchase;
            }

            // only need to finalize if on Android unless you turn off auto finalize on iOS
            var purchaseIdSuccess = await CrossInAppBilling.Current
                                                           .FinalizePurchaseAsync(purchase.TransactionIdentifier)
                                                           .ConfigureAwait(false);

            return purchaseIdSuccess;
            // Handle if acknowledge was successful or not

        }
        catch (InAppBillingPurchaseException purchaseEx)
        {
            //Billing Exception handle this based on the type
            Debug.WriteLine("Error: " + purchaseEx);
        }
        catch (Exception ex)
        {
            //Something else has gone wrong, log it
            Debug.WriteLine("Issue connecting: " + ex);
        }
        finally
        {
            await billing.DisconnectAsync();
        }

        return emptyPurchase;
    }
}
