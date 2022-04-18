using UnityEngine;

public class Flower : Item
{
    new public string name = "Flower";
    new public int price = 3;
    public new Sprite icon = Resources.Load("flower", typeof(Sprite)) as Sprite;  // color shader ?

    Flower(int colorSet, int intensitySet)
    {
        name = intensityToString(intensitySet) + " " + colorToString(colorSet) + " Flower";
        icon = Resources.Load("flower", typeof(Sprite)) as Sprite;  // color shader  // use .color 
        Color = colorSet;
        Intensity = intensitySet;
    }

    public override void Use()
    {
        // Replant
        Debug.Log("Using " + name);
    }
}
