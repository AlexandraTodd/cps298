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

    //private void Start()
    //{
    //    Flower test = Flower.CreateInstance(4, 1);
    //    Add(test);
    //}

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

    public void Add(Item itemData)   // there has got to be a way for this to be item
    {
        if (itemDictionary.TryGetValue(itemData.name, out InventorySlot value))
        {
            value.AddToStack();
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
            if (value.item.stackSize == 0)
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