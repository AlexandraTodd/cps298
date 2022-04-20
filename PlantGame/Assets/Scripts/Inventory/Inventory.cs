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

            // Note from Drake: For Unity, GameObjects are created in-game using static method Instantiate rather than made a new class object
            // InventorySlot newSlot = new InventorySlot(itemData);

            // Rather than creating a new class with the itemData as a parameter, we're going to Instantiate it in Unity and then call a method to update it
            // with the itemData as a parameter instead
            GameObject newSlotObject = Instantiate(inventorySlotPrefab, transform.position, Quaternion.identity);
            InventorySlot newInventorySlot = newSlotObject.GetComponent<InventorySlot>();
            newInventorySlot.Configure(itemData);
            Debug.Log("Adding " + newInventorySlot.item.name);
            items.Add(newInventorySlot);
            itemDictionary.Add(itemData.name, newInventorySlot);
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