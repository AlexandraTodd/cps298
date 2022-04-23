using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject inventorySlotPrefab;

    #region Singleton
    public static Inventory instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;
        items = new List<Item>();
        items = generateItemList();
        currency = generateCurrency();
    }
    #endregion

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))  // for testing, can be removed
        {
            Flower test = Flower.CreateInstance(4, 1);
            items[getItemNumber(test)].Buy();
            Flower test2 = Flower.CreateInstance(8, 2);
            Add(test2);
            Seed test3 = Seed.CreateInstance(10);
            Add(test3);
            Seed test4 = Seed.CreateInstance(5);
            Add(test4);
            Flower test5 = Flower.CreateInstance(9, 0);
            Add(test5);
        }
        if (Input.GetKeyDown(KeyCode.F))  // for testing, can be removed
        {
            Flower test = Flower.CreateInstance(4, 1);
            items[getItemNumber(test)].Sell();
            Seed test4 = Seed.CreateInstance(5);
            Remove(test4);
        }
        }

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;
    public List<Item> items;
    private int colorCount = 12;
    private int shadeCount = 3;
    private int currencyStartingAmount = 25;
    private int currency;

    public void Add(Item itemData)
    {
        items[getItemNumber(itemData)].AddToStack();
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }
    public void Remove(Item itemData)
    {
        if (items[getItemNumber(itemData)].stackSize > 0)
        {
            items[getItemNumber(itemData)].RemoveFromStack();
        }
        else
        {
            Debug.Log("No " + itemData.name + " to remove.");
        }
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    public void Buy(Item itemData)
    {
        items[getItemNumber(itemData)].Buy();
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }
    public void Sell(Item itemData)
    {
        if (items[getItemNumber(itemData)].stackSize > 0)
        {
            items[getItemNumber(itemData)].Sell();
        }
        else
        {
            Debug.Log("No " + itemData.name + " to remove.");
        }
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }


    private List<Item> generateItemList()   // change to if fresh save generate, else load list
    {
        int itemCount = 0;
        List<Item> freshItemList = new List<Item>();
        for (int c = 0; c < colorCount; c++) // c for color
        {
            for (int i = 0; i < shadeCount; i++) // i for intensity
            {
                freshItemList.Add(Flower.CreateInstance(c, i, itemCount));
                itemCount++;
            }
        }
        for (int c = 0; c < colorCount; c++) // c for color
        {
            freshItemList.Add(Seed.CreateInstance(c, itemCount));
            itemCount++;
        }
        return freshItemList;
    }
    public int getItemNumber(Item itemSet)
    {
        if (itemSet.itemType == 0)
        {
            return (itemSet.color * shadeCount - 1) + itemSet.intensity + 1;
        }
        else
        {
            return itemSet.color + (colorCount * shadeCount - 1);
        }
    }

    private int generateCurrency()   // generate on fresh save, load otherwise
    {
        return currencyStartingAmount;
    }
    public void addCurrency(int currencyInc)
    {
        currency += currencyInc;
        Debug.Log("Currency total incremented to: " + currency);
    }
    public void removeCurrency(int currencyDec)
    {
        currency -= currencyDec;
        Debug.Log("Currency total decremented to: " + currency);
    }
    public int getCurrency()
    {
        return currency;
    }
}