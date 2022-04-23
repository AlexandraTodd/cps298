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
        data.price = 1;
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
        data.price = 1;
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
}