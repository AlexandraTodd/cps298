using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootPointer : MonoBehaviour {
    public SpriteRenderer[] spriteRenderers;

    public void UpdateSpriteRendererColors(Color color) {
        foreach (SpriteRenderer sr in spriteRenderers) {
            sr.color = color;
        }
    }
}
