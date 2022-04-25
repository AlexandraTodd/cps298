using UnityEngine;

public class Item : ScriptableObject
{
    public int id;
    new public string name = "New Item";
    public Sprite icon;
    public int color = 0;
    public int intensity = 2;
    public int price = 1;
    public bool showInInventory = true;
    public int itemType = 0;
    public int stackSize = 0;

    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }

    public virtual void Use ()
    {
        Debug.Log("Using " + name);
        RemoveFromStack();
    }

    public void Sell()
    {
        Debug.Log("Selling " + name);
        if (this.stackSize > 0)
        {
            InventoryUI.SetNotificationText("Sold back " + name + "! You got back $" + this.price + ".");
            InventoryUI.Kaching();
            Inventory.instance.addCurrency(this.price);
            RemoveFromStack();
        }
        else
        {
            InventoryUI.SetNotificationText("You don't have "+name+" to sell back.");
            Debug.Log("No " + name + " to sell ");
        }
    }

    public void Buy()
    {
        Debug.Log("Buying " + name);
        if (Inventory.instance.getCurrency() >= price)
        {
            InventoryUI.SetNotificationText("You bought " + name + " for $"+this.price+".");
            InventoryUI.Kaching();
            Inventory.instance.removeCurrency(price);
            AddToStack();

            // Tell game we've bought an item, so unlock the flower rows
            if (PauseMenu.Instance != null) {
                PauseMenu.Instance.playerBoughtFirstTime = true;
                PauseMenu.Instance.showShopInstructions = false;
            }
        }
        else
        {
            InventoryUI.SetNotificationText("You don't have enough money to buy this " + name + "!");
            Debug.Log("Not enough currency to purchase " + name);
        }
    }

    public string colorToString(int colorInt)
    {
        switch (colorInt)
        {
            case 0:
                return "Red";
            case 1:
                return "Sunset";
            case 2:
                return "Orange";
            case 3:
                return "Amber";
            case 4:
                return "Yellow";
            case 5:
                return "Chartreuse";
            case 6:
                return "Green";
            case 7:
                return "Cyan";
            case 8:
                return "Blue";
            case 9:
                return "Iris";
            case 10:
                return "Violet";
            case 11:
                return "Fuchsia";
            default:
                return "Unknown Color";
        }
    }

    public string intensityToString(int intensityInt)  // change shader ?
    {
        switch(intensityInt)
        {
            case 0:
                return "Dark";
            case 1:
                return "Bright";
            case 2:
                return "Pastel";
            default:
                return "Unknown Intensity";
        }
    }
}
