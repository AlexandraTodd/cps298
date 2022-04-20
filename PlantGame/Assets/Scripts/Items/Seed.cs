using UnityEngine;

public class Seed : Item
{
    public string seedName = "Seed";  // add color ?
    public Sprite seedIcon = Resources.Load("seed", typeof(Sprite)) as Sprite;  // color shader ?

    Seed(int colorSet, int intensitySet)
    {
        seedName = intensityToString(intensitySet) + " " + colorToString(colorSet) + " Seed";
        seedIcon = Resources.Load("seed", typeof(Sprite)) as Sprite;  // color shader 
        Color = colorSet;
        Intensity = intensitySet;
    }

    public override void Use()
    {
        // Mini Game
        // Add to Planter
        Debug.Log("Using " + seedName);
    }
}
