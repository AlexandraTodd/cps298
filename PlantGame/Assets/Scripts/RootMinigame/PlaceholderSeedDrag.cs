using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaceholderSeedDrag : MonoBehaviour {
    public int colorIndex = -1;
    public Image icon;
    public int stackSize = 0;
    public TMP_Text stackText;
    public TMP_Text seedName;

    public void SetColor(int index) {
        colorIndex = index;

        // Changing .material directly causes ALL instances of this material to change its property
        // So we create a clone of it here
        Material clonedMaterial = new Material(icon.material);
        icon.material = clonedMaterial;
        icon.material.SetInt("inputColor", colorIndex);
    }

    void OnMouseDown() {
        RootMinigameManager.Instance.cursorGrabbed = colorIndex;
        RootMinigameManager.Instance.cursorImage.sprite = icon.sprite;

        // Changing .material directly causes ALL instances of this material to change its property
        // So we create a clone of it here
        Material clonedMaterial = new Material(RootMinigameManager.Instance.cursorImage.material);
        RootMinigameManager.Instance.cursorImage.material = clonedMaterial;
        RootMinigameManager.Instance.cursorImage.material.SetInt("inputColor", colorIndex);
    }
}
