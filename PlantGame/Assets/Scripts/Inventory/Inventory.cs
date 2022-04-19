using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
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
        itemDictionary = new Dictionary<string, InventorySlot>();
        items = new List<InventorySlot>();
    }
    #endregion

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Flower test = Flower.CreateInstance(4, 1);
            Add(test);
            Flower test2 = Flower.CreateInstance(8, 2);
            Add(test2);
            Seed test3 = Seed.CreateInstance(10);
            Add(test3);
        }
    }

    public InventorySlot Get(Item itemData)
    {
        if (itemDictionary.TryGetValue(itemData.name, out InventorySlot value))
        {
            return value;
        }
        return null;
    }

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;  // Trigger
    private Dictionary<string, InventorySlot> itemDictionary;   // testing
    public List<InventorySlot> items { get; private set; }
    //public Currency money;   // testing

    public void Add(Item itemData)
    {
        if (itemDictionary.TryGetValue(itemData.name, out InventorySlot value))
        {
            value.AddToStack();
            Debug.Log(itemData.name + " stack size: " + value.stackSize);
        }
        else
        {
            //GameObject newInstance = Instantiate(InventoryUI.Instance.inventorySlotPrefab);
            //InventorySlot newSlot = newInstance.AddComponent<InventorySlot>();
            //newSlot.AddItem(itemData);
            InventorySlot newSlot = new InventorySlot(itemData);
            Debug.Log("Adding " + newSlot.item.name);
            items.Add(newSlot);
            itemDictionary.Add(itemData.name, newSlot);
        }
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    public void Remove(Item itemData)
    {
        if (itemDictionary.TryGetValue(itemData.name, out InventorySlot value))
        {
            value.RemoveFromStack();
            if (value.stackSize == 0)
            {
                value.ClearSlot();
                itemDictionary.Remove(itemData.name);
            }
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
}