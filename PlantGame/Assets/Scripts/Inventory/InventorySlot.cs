using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image Icon;
    public Item item;
    public int stackSize { get; private set; }   // testing

    //public InventorySlot(Item newItem)
    //{
    //    item = newItem;
    //    Icon.sprite = item.icon;
    //    Icon.enabled = true;
    //    stackSize = 1;
    //}

    public void AddSlot(InventorySlot newSlot)  // edited
    {
        Icon.sprite = InventoryUI.Instance.flowerIcon;
        Icon.enabled = true;
        item = newSlot.item;
        stackSize = newSlot.stackSize;
    }

    public void AddItem(Item newItem)
    {
        Debug.Log("adding");
        item = newItem;
        Debug.Log("adding1");
        Debug.Log("item type: " + item.GetType());
        Debug.Log("adding2");
        //Debug.Log("flower sprite exists: ");
        //if (InventoryUI.Instance.flowerIcon != null)
        //{
        //    Debug.Log("exists");
        //    Debug.Log(InventoryUI.Instance.flowerIcon);
        //}
        //if (Icon != null)
        //{
        //    Debug.Log("icon.sprite exists");
        //    Debug.Log(Icon.sprite);
        //}
        Icon.sprite = InventoryUI.Instance.flowerIcon;
        Debug.Log("adding3");
        Icon.enabled = true;
        Debug.Log("adding4");
        Debug.Log("stack size: " + this.stackSize);
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
        Icon.sprite = null;
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
