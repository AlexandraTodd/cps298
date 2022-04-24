using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        // items = generateItemList();
        items = LoadInventoryItems();
        currency = generateCurrency();
    }
    #endregion

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))  // for testing, can be removed
        {
            Flower test = Flower.CreateInstance(4, 1);
            Add(test);
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
            Remove(test);
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
            freshItemList[itemCount].AddToStack();
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
    public int getCurrency() { return currency; }
    private void setCurrency(int currencySet) { currency = currencySet; }

    public void SaveInventoryItems() {
        BinaryFormatter formatter = new BinaryFormatter();

        // Inventory
        string path = Application.persistentDataPath + "/inventory.dat";
        FileStream stream = new FileStream(path, FileMode.Create);
        InventorySave data = new InventorySave(items);
        formatter.Serialize(stream, data);

        // Currency
        path = Application.persistentDataPath + "/currency.dat";
        stream = new FileStream(path, FileMode.Create);
        CurrencySave currencySave = new CurrencySave(getCurrency());
        formatter.Serialize(stream, data);

        // Done
        stream.Close();
    }

    public List<Item> LoadInventoryItems() {
        List<Item> reconstructedList = new List<Item>();

        // Will use old method to generate list if necessary
        bool generateNewList = true;

        // Currency is accessed more frequently than inventory items, so currency will be going in a separate file
        string currencyPath = Application.persistentDataPath + "/currency.dat";
        if (File.Exists(currencyPath)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(currencyPath, FileMode.Open);
            if (stream.Length != 0) {
                CurrencySave data = (CurrencySave)(formatter.Deserialize(stream));
                setCurrency(data.currency);
            }
        }

        // Load inventory items
        string path = Application.persistentDataPath + "/inventory.dat";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length != 0) {
                // This loads in a binary file and reconstructs its integer arrays into List items
                InventorySave data = (InventorySave)(formatter.Deserialize(stream));
                int[] itemListType = data.itemListType;
                int[] itemListStack = data.itemListStack;
                int[] itemListColor = data.itemListColor;
                int[] itemListIntensity = data.itemListIntensity;

                for (int i = 0; i < itemListType.Length; i++) {
                    // This is only here to make the compiler shut up
                    Item newItem = ScriptableObject.CreateInstance<Item>();

                    // We have inventory items already, so no need to generate a list of every index
                    generateNewList = false;

                    // Some properties such as price are defined by the CreateInstance method of items rather than in an index, so we need to call a method based on type
                    switch (itemListType[i]) {
                        // 0 = flower
                        case 0:
                            newItem = Flower.CreateInstance(itemListColor[i], itemListIntensity[i]);
                            break;

                        // 1 = seed
                        case 1:
                            newItem = Seed.CreateInstance(itemListColor[i]);
                            break;

                        default:
                            Debug.LogError("Unreadable item type " + itemListType[i] + " loaded");
                            break;
                    }

                    newItem.stackSize = itemListStack[i];
                    reconstructedList.Add(newItem);
                }
                stream.Close();
                Debug.Log("Inventory file loaded");
            }
        }

        // Calls old method to ensure everything has a stack available
        if (generateNewList) {
            reconstructedList = generateItemList();
            Debug.Log("Inventory file was not found or did not load propertly, creating new blank list instead");
        }

        // Automatically returns an empty list if there's no save
        return reconstructedList;
    }
}