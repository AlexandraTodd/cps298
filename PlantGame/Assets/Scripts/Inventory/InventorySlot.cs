using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image Icon;
    Item item;

    public void AddItem(Item newItem)
    {
        item = newItem;
        Icon.sprite = item.itemIcon;
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
        Icon.sprite = null;
        Icon.enabled = false;
    }

    public void UseItem ()
    {
        if (item != null)
        {
            item.Use();
        }
    }
}
