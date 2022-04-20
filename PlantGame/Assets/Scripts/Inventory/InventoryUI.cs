using UnityEngine;

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
    public static GameObject go;
    //public GameObject inventorySlotPrefab;

    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        FlowerSlots = FlowerItemsParent.GetComponentsInChildren<InventorySlot>();
        SeedSlots = SeedItemsParent.GetComponentsInChildren<InventorySlot>();
        FlowerInventoryUI.SetActive(true);    // testing
        go = new GameObject();
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
            if(inventory.items[i].item.itemType == 0)
            {
                FlowerSlots[flowers].AddSlot(inventory.items[i]);
                flowers++;
            }
            else
            {
                SeedSlots[seeds].AddSlot(inventory.items[i]);
                seeds++;
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

        Debug.Log("Updating UI");
    }
}
