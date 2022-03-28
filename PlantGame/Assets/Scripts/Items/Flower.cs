using UnityEngine;

public class Flower : Item
{
    new public string name = "Flower";  // add color ?
    public int price = 5;
    public new Sprite icon = Resources.Load("flower", typeof(Sprite)) as Sprite;  // color shader ?

    Flower(int colorSet, int intensitySet)
    {
        name = intensityToString(intensitySet) + " " + colorToString(colorSet) + " Flower";
        icon = Resources.Load("flower", typeof(Sprite)) as Sprite;  // color shader 
        Color = colorSet;
        Intensity = intensitySet;
    }

    public override void Use()
    {
        // Sell
        Debug.Log("Using " + name);
    }
}
