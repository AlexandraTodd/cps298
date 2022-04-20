using UnityEngine;

[CreateAssetMenu(fileName = "New Flower", menuName = "Inventory/Flower")]
public class Flower : Item
{
    public int intensity = 1;

    public static Flower CreateInstance(int colorSet, int intensitySet)
    {
        Flower data = ScriptableObject.CreateInstance<Flower>();
        data.name = data.intensityToString(intensitySet) + " " + data.colorToString(colorSet) + " Flower";
        data.color = colorSet;
        data.intensity = intensitySet;
        data.price = 3 + intensitySet;
        data.itemType = 0;
        return data;
    }

    public override void Use()
    {
        // Replant
        Debug.Log("Using " + name);
    }
}
