using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image Icon;
    public Item item;
    public int stackSize;
    private string stackSizeString;
    public Text stackSizeText;
    private string itemNameString;
    public Text itemNameText;
    //public Material itemMaterial;

    // Note from Drake:
    // This does not get used. Instantiating Unity MonoBehaviour objects behave differently than creating instances of classes and use Awake()/Start() instead
    /*
    public InventorySlot(Item newItem)
    {
        item = newItem;
        stackSize = 1;
        string stackSizeString = stackSize.ToString();
        //Text newText = InventoryUI.Instance.GetComponent<Text>();
        //Debug.Log(newText);
        //stackSizeText = newText;
        //stackSizeText.text = stackSizeString;
        //stackSizeText.GetComponent<Text>().text = stackSizeString;
        //stackSizeText = GetComponent<UnityEngine.UI.Text>();

        //InventoryUI.go.AddComponent<Text>();
        //stackSizeText = InventoryUI.go.GetComponent<Text>();
        //stackSizeText.text = stackSize.ToString();

        // Note from Drake: For Unity, GameObjects are created in-game using static method Instantiate rather than made a new class object
        // GameObject go = new GameObject();

        // GameObject go = Instantiate()
        // stackSizeText = go.GetComponent<Text>();
        stackSizeText.text = stackSize.ToString();
    }
    */

    public void Configure(Item itemData) {
        item = itemData;
        stackSize = 1;
        stackSizeString = stackSize.ToString();
        itemNameString = item.name;
    }

    public void AddSlot(InventorySlot newSlot)
    {
        item = newSlot.item;
        Icon.enabled = true;
        //Icon.material = 
        stackSize = newSlot.stackSize;
        stackSizeText.text = newSlot.stackSizeString;
        itemNameText.text = newSlot.itemNameString;
    }

    public void AddToStack()   // testing
    {
        stackSize++;
        stackSizeString = stackSize.ToString();
        stackSizeText.text = stackSizeString;
    }

    public void RemoveFromStack()   // testing
    {
        stackSize--;
        stackSizeString = stackSize.ToString();
        stackSizeText.text = stackSizeString;
    }

    public void ClearSlot ()
    {
        item = null;
        Icon.enabled = false;
        //stackSizeText.enabled = false;
    }

    public void UseItem ()  // shouldn't be in here, should go thru inventory.remove
    {
        if (item != null)
        {
            item.Use();
            RemoveFromStack();   // testing
        }
    }
}
