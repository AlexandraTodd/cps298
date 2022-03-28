using UnityEngine;

public class Seed : Item
{
    new public string name = "Seed";  // add color ?
    public new Sprite icon = Resources.Load("seed", typeof(Sprite)) as Sprite;  // color shader ?

    Seed(int colorSet, int intensitySet)
    {
        name = intensityToString(intensitySet) + " " + colorToString(colorSet) + " Seed";
        icon = Resources.Load("seed", typeof(Sprite)) as Sprite;  // color shader 
        Color = colorSet;
        Intensity = intensitySet;
    }

    public override void Use()
    {
        // Mini Game
        // Add to Planter
        Debug.Log("Using " + name);
    }
}
