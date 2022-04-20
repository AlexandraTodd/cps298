using UnityEngine;

public class Flower : Item
{
    public string flowerName = "Flower";  // add color ?
    public int price = 5;
    public Sprite flowerIcon = Resources.Load("flower", typeof(Sprite)) as Sprite;  // color shader ?

    Flower(int colorSet, int intensitySet)
    {
        flowerName = intensityToString(intensitySet) + " " + colorToString(colorSet) + " Flower";
        flowerIcon = Resources.Load("flower", typeof(Sprite)) as Sprite;  // color shader 
        Color = colorSet;
        Intensity = intensitySet;
    }

    public override void Use()
    {
        // Sell
        Debug.Log("Using " + flowerName);
    }
}
