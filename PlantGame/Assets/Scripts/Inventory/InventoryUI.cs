using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;
    public Transform ItemsParent;
    public GameObject inventoryUI;
    Inventory inventory;
    public InventorySlot[] slots;
    //public GameObject inventorySlotPrefab;

    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        slots = ItemsParent.GetComponentsInChildren<InventorySlot>();
        inventoryUI.SetActive(true);    // testing
        UpdateUI();
    }

    void Awake()
    {
        Instance = this;
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddSlot(inventory.items[i]);  // edited
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
        Debug.Log("Updating UI");
    }
}
