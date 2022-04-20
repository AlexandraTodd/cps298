using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image Icon;
    public Item item;
    public int stackSize;
    public Text stackSizeText;

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
        GameObject go = new GameObject();
        stackSizeText = go.GetComponent<Text>();
        stackSizeText.text = stackSize.ToString();
    }

    public void AddSlot(InventorySlot newSlot)  // edited
    {
        item = newSlot.item;
        Icon.enabled = true;
        stackSize = newSlot.stackSize;
       // stackSizeText = newSlot.stackSizeText;
       // stackSizeText.enabled = true;
    }

    public void AddToStack()   // testing
    {
        stackSize++;
    }

    public void RemoveFromStack()   // testing
    {
        stackSize--;
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
