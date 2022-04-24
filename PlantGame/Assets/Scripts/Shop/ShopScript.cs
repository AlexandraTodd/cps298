using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopScript : MonoBehaviour
{
    public bool isOpen;

    public void OpenShop()
    {
        if(!isOpen)
        {
            isOpen = true;
            Debug.Log("Shop is now open...");
            SceneManager.LoadScene("InventoryFlower");
        }
    }
}
