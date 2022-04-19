using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image Icon;
    public Item item;

    public InventorySlot(Item newItem)
    {
        item = newItem;
    }

    public void AddSlot(InventorySlot newSlot)  // edited
    {
        item = newSlot.item;
        Icon.enabled = true;
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
    }

    public void AddToStack()   // testing
    {
        item.stackSize++;
    }

    public void RemoveFromStack()   // testing
    {
        item.stackSize--;
    }

    public void ClearSlot ()
    {
        item = null;
        Icon.enabled = false;
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
