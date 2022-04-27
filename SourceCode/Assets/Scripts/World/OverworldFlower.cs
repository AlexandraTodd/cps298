using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldFlower : MonoBehaviour {
    public SpriteRenderer spriteRenderer;

    public void SetColorIntensity(int colorIndex, int intensity) {
        spriteRenderer.material.SetInt("inputColor", colorIndex);
        spriteRenderer.material.SetInt("inputIntensity", intensity);
    }
}
