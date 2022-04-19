using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image Icon;
    public Item item;
    public int stackSize;

    public InventorySlot(Item newItem)
    {
        item = newItem;
    }

    public void AddSlot(InventorySlot newSlot)  // edited
    {
        item = newSlot.item;
        Icon.enabled = true;
        stackSize = newSlot.stackSize;
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
        stackSize = 1;
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
