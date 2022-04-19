using UnityEngine;

//[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;  // seed or flower
    public int color = 0;
    public int price = 1;
    public bool showInInventory = true;
    // genes ?

    public virtual void Use ()
    {
        Debug.Log("Using " + name);
    }

    public void Sell ()
    {
        Debug.Log("Selling " + name);
        // add price to currency
        //money.gain(price);
        RemoveFromInventory();
    }

    public void RemoveFromInventory ()
    {
        Inventory.instance.Remove(this);
    }

    public string colorToString(int colorInt)
    {
        switch (colorInt)
        {
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
                return "Red";
        }
    }

    public string intensityToString(int intensityInt)
    {
        switch(intensityInt)
        {
            case 0:
                return "Pastel";
            case 2:
                return "Bright";
            default:
                return "Dark";

        }
    }
}
