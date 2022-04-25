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
        switch (seedColor) {
            // Primary colors are 1
            case 0:
            case 4:
            case 8:
                return 1;

            // Secondary(?) colors are 3
            case 2:
            case 6:
            case 10:
                return 3;

            // Tertiary(?) colors are 5
            default:
                return 5;
        }

        /*
        int seedPrice = 1;
        if(seedColor == 0 || seedColor % 2 == 0) { } // roygbp
        else
        {
            seedPrice = 3;
        }
        return seedPrice;
        */
    }
}