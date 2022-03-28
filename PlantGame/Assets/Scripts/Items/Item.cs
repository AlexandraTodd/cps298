using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    public Sprite icon = null;  // seed or flower
    public int Color = 0;
    public int Intensity = 1;
    public bool showInInventory = true;
    // Gene ga;
    // Gene gb;

    public virtual void Use ()
    {
        Debug.Log("Using " + name);
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
                return "Orange";
            case 2:
                return "Yellow";
            case 3:
                return "Green";
            case 4:
                return "Blue";
            case 5:
                return "Purple";
            default:
                return "Red";
        }
    }

    public string intensityToString(int intensityInt)
    {
        switch(intensityInt)
        {
            case 0:
                return "Light";
            case 2:
                return "Dark";
            default:
                return "Bright";

        }
    }
}
