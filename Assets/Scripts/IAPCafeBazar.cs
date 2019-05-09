using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BazaarPlugin;

public class IAPCafeBazar : MonoBehaviour
{
    public string RSA;
    public string[] skus;
    public int[] gem;//coin 
    public int[] amount;
    public GameObject panelWait, panelMessage, panelRetryCheck;
    public Text txtPanelMessage;
    // Use this for initialization
    void Start()
    {
        BazaarIAB.init(RSA);//در صورت موفقیت رخداد
    }
    public void BtnPurchase(string sku)
    {
        Debug.Log("btn Purchase: " + sku);
        BazaarIAB.purchaseProduct(sku);
    }
    public void BtnPurchaseEshterak(string sku)
    {
        //behtar hast ke az developerpayload estefade shavad.
        BazaarIAB.purchaseProduct(sku);
    }
}
