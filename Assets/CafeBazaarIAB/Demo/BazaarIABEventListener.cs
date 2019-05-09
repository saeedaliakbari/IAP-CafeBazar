using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BazaarPlugin;


public class BazaarIABEventListener : MonoBehaviour
{
    public IAPCafeBazar iapCafeBazar;
    private string[] str;
    private BazaarPurchase currentPurchase;
#if UNITY_ANDROID
    void OnEnable()
	{
		// Listen to all events for illustration purposes
		IABEventManager.billingSupportedEvent += billingSupportedEvent;
		IABEventManager.billingNotSupportedEvent += billingNotSupportedEvent;
		IABEventManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		IABEventManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
        IABEventManager.querySkuDetailsSucceededEvent += querySkuDetailsSucceededEvent;
        IABEventManager.querySkuDetailsFailedEvent += querySkuDetailsFailedEvent;
        IABEventManager.queryPurchasesSucceededEvent += queryPurchasesSucceededEvent;
        IABEventManager.queryPurchasesFailedEvent += queryPurchasesFailedEvent;
        IABEventManager.purchaseSucceededEvent += purchaseSucceededEvent;
		IABEventManager.purchaseFailedEvent += purchaseFailedEvent;
		IABEventManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
		IABEventManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
	}
    void OnDisable()
	{
		// Remove all event handlers
		IABEventManager.billingSupportedEvent -= billingSupportedEvent;
		IABEventManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		IABEventManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		IABEventManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
        IABEventManager.querySkuDetailsSucceededEvent -= querySkuDetailsSucceededEvent;
        IABEventManager.querySkuDetailsFailedEvent -= querySkuDetailsFailedEvent;
        IABEventManager.queryPurchasesSucceededEvent -= queryPurchasesSucceededEvent;
        IABEventManager.queryPurchasesFailedEvent -= queryPurchasesFailedEvent;
        IABEventManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		IABEventManager.purchaseFailedEvent -= purchaseFailedEvent;
		IABEventManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		IABEventManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
	}
    #region Init
    void billingSupportedEvent()
	{
		Debug.Log("billingSupportedEvent");
        str = new string[iapCafeBazar.skus.Length];
        for (int i = 0; i < iapCafeBazar.skus.Length; i++)
        {
            str[i] = iapCafeBazar.skus[i];
        }
        BazaarIAB.queryInventory(str);
    }

	void billingNotSupportedEvent(string error)
	{
		Debug.Log("billingNotSupportedEvent: " + error);
	}
    #endregion
    #region Inventory
    void queryInventorySucceededEvent(List<BazaarPurchase> purchases, List<BazaarSkuInfo> skus)
	{
		Debug.Log(string.Format("queryInventorySucceededEvent. total purchases: {0}, total skus: {1}", purchases.Count, skus.Count));

        for (int i = 0; i < purchases.Count; ++i)
        {
            //handle purchase
            Debug.Log(purchases[i].ToString());
        }

        Debug.Log("-----------------------------");

        for (int i = 0; i < skus.Count; ++i)
        {
            Debug.Log(skus[i].ToString());
        }
    }

	void queryInventoryFailedEvent(string error)
	{
		Debug.Log("queryInventoryFailedEvent: " + error);
        //retry query inventory
        BazaarIAB.queryInventory(str);
    }
    #endregion
    #region Sku Details
    private void querySkuDetailsSucceededEvent(List<BazaarSkuInfo> skus)
    {
        Debug.Log(string.Format("querySkuDetailsSucceededEvent. total skus: {0}", skus.Count));

        for (int i = 0; i < skus.Count; ++i)
        {
            Debug.Log(skus[i].ToString());
        }
    }

    private void querySkuDetailsFailedEvent(string error)
    {
        Debug.Log("querySkuDetailsFailedEvent: " + error);
    }
    #endregion
    #region Multiple Purchase
    private void queryPurchasesSucceededEvent(List<BazaarPurchase> purchases)
    {
        Debug.Log(string.Format("queryPurchasesSucceededEvent. total purchases: {0}", purchases.Count));

        for (int i = 0; i < purchases.Count; ++i)
        {
            Debug.Log(purchases[i].ToString());
        }
    }

    private void queryPurchasesFailedEvent(string error)
    {
        Debug.Log("queryPurchasesFailedEvent: " + error);
    }
    #endregion
    #region Purchase
    void purchaseSucceededEvent(BazaarPurchase purchase)
	{
        currentPurchase = purchase;
        Debug.Log("purchaseSucceededEvent: " + purchase);
        if (purchase.PurchaseState == BazaarPurchase.BazaarPurchaseState.Purchased)
        {
            iapCafeBazar.panelWait.SetActive(true);
            GetComponent<CheckIABValidate>().check(purchase, onPurchaseValidated);
        }
        else if (purchase.PurchaseState == BazaarPurchase.BazaarPurchaseState.Canceled)
        {
            Debug.Log("purchase is Canceled");
            iapCafeBazar.panelMessage.SetActive(true);
            iapCafeBazar.txtPanelMessage.text = "عملیات توسط شما لغو شد";
        }
        else
        {
            Debug.Log("purchase is 2 Refunded");
            iapCafeBazar.panelMessage.SetActive(true);
            iapCafeBazar.txtPanelMessage.text = "خطا در عملیات پرداخت";
        }
    }

	void purchaseFailedEvent(string error)
	{
		Debug.Log("purchaseFailedEvent: " + error);
        iapCafeBazar.txtPanelMessage.text = "خطا در پرداخت ";
        iapCafeBazar.panelMessage.SetActive(true);
    }
    #endregion
    #region CheckPurchase
    private void onPurchaseValidated(bool success, string message, validateResult result)
    {
        if (success)
        {
            if (!result.isRefund) // اگر خرید از طرف کافه‌بازار برگشت داده نشده
            {
                Debug.Log("purchase is not refund" + message);
                //Handle purchase
            }
            else
            {
                iapCafeBazar.panelMessage.SetActive(true);
                iapCafeBazar.txtPanelMessage.text = "خرید ناموفق/nپول کسر شده از حساب شما برگشت داده می شود";
                Debug.Log("the purchase is refund");
            }
        }
        else
        {
            // error in validating, or purchase is not valid
            // you can let user retry validating the purchase
            iapCafeBazar.panelRetryCheck.SetActive(true);
            Debug.Log(message);
        }
        // hide loading here
        iapCafeBazar.panelWait.SetActive(false);
    }

    public void retryValidatePurchase()
    {
        iapCafeBazar.panelRetryCheck.SetActive(false);
        iapCafeBazar.panelWait.SetActive(true);
        GetComponent<CheckIABValidate>().check(currentPurchase, onPurchaseValidated);
    }
    #endregion
    #region Consume
    void consumePurchaseSucceededEvent(BazaarPurchase purchase)
    {
        Debug.Log("consumePurchaseSucceededEvent: " + purchase);
        for (int i = 0; i < iapCafeBazar.skus.Length; i++)
        {
            //handle consume items
        }
        iapCafeBazar.panelWait.SetActive(false);
    }
	void consumePurchaseFailedEvent(string error)
	{
		Debug.Log("consumePurchaseFailedEvent: " + error);
        BazaarIAB.queryInventory(str);
        iapCafeBazar.panelWait.SetActive(false);
    }
    #endregion
#endif

}
public class validateResult
{
    public bool isConsumed;
    public bool isRefund;
    public string kind;
    public string payload;
    public string time;
}

