using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trading : MonoBehaviour
{
    [SerializeField] GameObject storePanel;
    [SerializeField] GameObject inventoryPanel;

    Store store;

    public void BeginTrading(Store store)
    {
        this.store = store;
        
        storePanel.SetActive(true);
        inventoryPanel.SetActive(true);
        
    }
}