using UnityEngine;

[CreateAssetMenu(fileName = "New Flower", menuName = "Inventory/Flower")]
public class Flower : Item
{
    public int intensity = 1;

    //Flower(int colorSet, int intensitySet)
    //{
    //    name = intensityToString(intensitySet) + " " + colorToString(colorSet) + " Flower";
    //    icon = Resources.Load<Sprite>("flower");  // colorshader..
    //    color = colorSet;
    //    intensity = intensitySet;
    //    price = 3 + intensitySet;
    //}

    public static Flower CreateInstance(int colorSet, int intensitySet)
    {
        Flower data = ScriptableObject.CreateInstance<Flower>();
        data.name = data.intensityToString(intensitySet) + " " + data.colorToString(colorSet) + " Flower";
        data.icon = InventoryUI.Instance.flowerIcon;  // color shader  // use .color 
        data.color = colorSet;
        data.intensity = intensitySet;
        data.price = 3 + intensitySet;
        return data;
    }

    public override void Use()
    {
        // Replant
        Debug.Log("Using " + name);
    }
}
