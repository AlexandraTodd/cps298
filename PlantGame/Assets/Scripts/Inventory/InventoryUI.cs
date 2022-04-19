using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;
    public Transform ItemsParent;
    public GameObject inventoryUI;
    Inventory inventory;
    public InventorySlot[] slots;
    public GameObject inventorySlotPrefab;
    public Sprite flowerIcon;
    public Sprite seedIcon;

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

    void Update()
    {
        //if(Input.GetButtonDown("Inventory"))
        //{
        //    inventoryUI.SetActive(!inventoryUI.activeSelf);
        //}
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                Debug.Log("inventory.items[i]" + inventory.items[i].item.name);
                slots[i].AddSlot(inventory.items[i]);  // edited
                Debug.Log("slots[i] " + slots[i].item.name);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
        Debug.Log("Updating UI");
    }
}
