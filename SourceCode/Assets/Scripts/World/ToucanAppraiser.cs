using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToucanAppraiser : MonoBehaviour {
    public SpriteRenderer highlightSprite;
    [HideInInspector] public float flashingAnimation = 0f;

    // Update is called once per frame
    void Update() {
        flashingAnimation += Time.deltaTime * 2f;

        // Plots of land with nutrients still available have a yellow glow that fades in and out
        Color rowAvailableColor = highlightSprite.color;
        rowAvailableColor.a = Mathf.Abs(Mathf.Sin(flashingAnimation)) * 0.25f;
        highlightSprite.color = rowAvailableColor;
    }

    void OnMouseDown() {
        if (OverworldManager.Instance.fadeOutAnimation == 0f) {
            OverworldManager.Instance.OpenAppraisalPrompt();
        }
    }
}
