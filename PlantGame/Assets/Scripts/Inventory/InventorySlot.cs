using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {
    public Image Icon;
    public Item item;
    public int stackSize;
    private string stackSizeString;
    public Text stackSizeText;
    private string itemNameString;
    public Text itemNameText;
    public Text priceText;

    public void LoadSlot(Item content)
    {
        item = content;
        Icon.enabled = true;
        Material clonedMaterial = new Material(Icon.material);
        Icon.material = clonedMaterial;
        Icon.material.SetInt("inputColor", item.color);
        Icon.material.SetInt("inputIntensity", item.intensity);
        stackSize = content.stackSize;
        stackSizeText.text = content.stackSize.ToString();
        itemNameText.text = content.name;
        priceText.text = "$" + content.price.ToString();
    }

    public void BuyButton()
    {
        if (item)
        {
            item.Buy();
            if (Inventory.instance.onItemChangedCallback != null)
            {
                Inventory.instance.onItemChangedCallback.Invoke();
            }
        }
        else
        {
            Debug.Log("nothing for sale here.");
        }
    }
    public void SellButton()
    {
        if (item)
        {
            item.Sell();
            if (Inventory.instance.onItemChangedCallback != null)
            {
                Inventory.instance.onItemChangedCallback.Invoke();
            }
        }
        else
        {
            Debug.Log("nothing for sale here.");
        }
    }

    public void ClearSlot ()
    {
        item = null;
        Icon.enabled = false;
        stackSizeText.text = "";
        itemNameText.text = "";
        priceText.text = "";
    }
}
