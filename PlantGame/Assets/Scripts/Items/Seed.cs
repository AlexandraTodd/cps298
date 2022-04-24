using UnityEngine;

[CreateAssetMenu(fileName = "New Seed", menuName = "Inventory/Seed")]
public class Seed : Item
{
    public static Seed CreateInstance(int colorSet)
    {
        Seed data = ScriptableObject.CreateInstance<Seed>();
        data.name = data.colorToString(colorSet) + " Seed";
        data.color = colorSet;
        data.intensity = 2;
        data.price = data.setSeedPrice(data.color);
        data.itemType = 1;
        data.stackSize = 1;
        return data;
    }
    public static Seed CreateInstance(int colorSet, int idSet)
    {
        Seed data = ScriptableObject.CreateInstance<Seed>();
        data.id = idSet;
        data.name = data.colorToString(colorSet) + " Seed";
        data.color = colorSet;
        data.intensity = 2;
        data.price = data.setSeedPrice(data.color);
        data.itemType = 1;
        data.stackSize = 0;
        return data;
    }

    public override void Use()
    {
        // Mini Game
        // Add to Planter
        Debug.Log("Using " + name);
    }

    private int setSeedPrice(int seedColor)
    {
        int seedPrice = 1;
        if(seedColor == 0 || seedColor % 2 == 0) { } // roygbp
        else
        {
            seedPrice = 3;
        }
        return seedPrice;
    }
}