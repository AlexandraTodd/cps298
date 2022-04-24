using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;
    public Transform FlowerItemsParent;
    public Transform SeedItemsParent;
    public GameObject FlowerInventoryUI;
    public GameObject SeedInventoryUI;
    Inventory inventory;
    public InventorySlot[] FlowerSlots;
    public InventorySlot[] SeedSlots;
    public Text currencyText;
    public int[] shopSeeds = { 0, 4, 8 };

    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        FlowerSlots = FlowerItemsParent.GetComponentsInChildren<InventorySlot>();
        SeedSlots = SeedItemsParent.GetComponentsInChildren<InventorySlot>();
        SeedInventoryUI.SetActive(true);
        UpdateUI();
    }

    void Awake()
    {
        Instance = this;
    }

    void UpdateUI()
    {
        int flowers = 0;
        int seeds = 0;
        for (int i = 0;  i < inventory.items.Count; i++)
        {
            if(inventory.items[i].itemType == 0)
            {
                if (inventory.items[i].stackSize > 0) // comment out this if statement and you can see all flowers in inventory to assess color
                {
                    FlowerSlots[flowers].LoadSlot(inventory.items[i]);
                    flowers++;
                }
            }
            else
            {
                if ((inventory.items[i].stackSize > 0) || checkIfShopSeed(inventory.items[i].color)) // comment out this if statement and you can see all seeds in inventory to assess color
                {
                    SeedSlots[seeds].LoadSlot(inventory.items[i]);
                    seeds++;
                }
            }
        }

        for (int i = flowers; i < FlowerSlots.Length; i++)
        {
            FlowerSlots[i].ClearSlot();
        }

        for (int i = seeds; i < SeedSlots.Length; i++)
        {
            SeedSlots[i].ClearSlot();
        }
        currencyText.text = "Currency: " + inventory.getCurrency().ToString();

        // From Drake: also save an inventory file
        // It may be ideal to do this on a less regular interval such as closing the menu, so I'm placing it in a separate method just in case
        inventory.SaveInventoryItems();
    }

    private bool checkIfShopSeed(int seedColor)
    {
        foreach(int seed in shopSeeds)
        {
            if(seed == seedColor)
            {
                return true;
            }
        }
        return false;
    }

    public void exitInventory()
    {
        SceneManager.LoadScene("TownMap");
    }
}
