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
        Inventory.instance.addCurrency(this.price);
        RemoveFromStack();
    }

    public void Buy()
    {
        Debug.Log("Buying " + name);
        if (Inventory.instance.getCurrency() >= price)
        {
            Inventory.instance.removeCurrency(price);
            AddToStack();
        }
        else
        {
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
                return "Red-Orange";
            case 2:
                return "Orange";
            case 3:
                return "Yellow-Orange";
            case 4:
                return "Yellow";
            case 5:
                return "Yellow-Green";
            case 6:
                return "Green";
            case 7:
                return "Blue-Green";
            case 8:
                return "Blue";
            case 9:
                return "Blue-Purple";
            case 10:
                return "Purple";
            case 11:
                return "Red-Purple";
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
