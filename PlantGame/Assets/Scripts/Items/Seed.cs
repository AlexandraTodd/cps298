using UnityEngine;

[CreateAssetMenu(fileName = "New Seed", menuName = "Inventory/Seed")]
public class Seed : Item
{
    // growth time ?

    //public static GenerateItem CreateInstance(ing colorSet)
    //{
    //    // You don't need "ScriptableObject." since this class inherits from it
    //    GenerateItem o = CreateInstance<GenerateItem>();

    //    o.name = colorToString(colorSet) + " Seed";
    //    o.icon = Resources.Load("seed", typeof(Sprite)) as Sprite;  // color shader
    //    o.color = colorSet;

    //    return o;
    //}

    public Seed(int colorSet)
    {
        name = colorToString(colorSet) + " Seed";
        icon = Resources.Load("seed", typeof(Sprite)) as Sprite;  // color shader 
        color = colorSet;
        //Debug.Log("name: " + name " color: " + color); //why doesn't this work
    }

    public override void Use()
    {
        // Mini Game
        // Add to Planter
        Debug.Log("Using " + name);
    }
}