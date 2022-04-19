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
        itemDictionary = new Dictionary<Item, InventorySlot>();
        items = new List<InventorySlot>();
    }
    #endregion

    private void Start()
    {
        //Flower test = Flower.CreateInstance(4, 1);
        //Add(test);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Flower test = Flower.CreateInstance(4, 1);
            Add(test);
        }
    }

    public InventorySlot Get(Item itemData)
    {
        if (itemDictionary.TryGetValue(itemData, out InventorySlot value))
        {
            return value;
        }
        return null;
    }

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;  // Trigger
    private Dictionary<Item, InventorySlot> itemDictionary;   // testing
    public List<InventorySlot> items { get; private set; }
    //public Currency money;   // testing

    //private void Awake()   // testing
    //{
    //    slots = new List<InventorySlot>();
    //    itemDictionary = new Dictionary<Item, InventorySlot>();
    //}

    public void Add(Flower itemData)   // there has got to be a way for this to be item
    {
        if (itemDictionary.TryGetValue(itemData, out InventorySlot value))
        {
            value.AddToStack();
        }
        else
        {
            GameObject newInstance = Instantiate(InventoryUI.Instance.inventorySlotPrefab);
            InventorySlot newSlot = newInstance.AddComponent<InventorySlot>();
            newSlot.AddItem(itemData);
            items.Add(newSlot);
            itemDictionary.Add(itemData, newSlot);
        }
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }
    public void Add(Seed itemData)
    {
        if (itemDictionary.TryGetValue(itemData, out InventorySlot value))
        {
            value.AddToStack();
        }
        else
        {
            GameObject newInstance = Instantiate(InventoryUI.Instance.inventorySlotPrefab);
            InventorySlot newSlot = newInstance.AddComponent<InventorySlot>();
            newSlot.AddItem(itemData);
            items.Add(newSlot);
            itemDictionary.Add(itemData, newSlot);
        }
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    public void Remove(Item itemData)
    {
        if (itemDictionary.TryGetValue(itemData, out InventorySlot value))
        {
            value.RemoveFromStack();
            if (value.stackSize == 0)
            {
                value.ClearSlot();
                itemDictionary.Remove(itemData);
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