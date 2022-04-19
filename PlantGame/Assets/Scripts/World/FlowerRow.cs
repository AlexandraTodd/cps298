using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class FlowerRow : MonoBehaviour {
    public SpriteRenderer highlightSprite;
    public int slotNumber;
    public GameObject flowerTestPrefab;
    [HideInInspector] public float flashingAnimation = 0f;

    void Awake() {
        highlightSprite.enabled = false;

        string path = Application.persistentDataPath + "/roots" + slotNumber + ".dat";
        if (File.Exists(path)) {

        }
    }

    void Update() {
        if (highlightSprite.enabled) {
            flashingAnimation += Time.deltaTime * 2f;
            Color flashingColor = highlightSprite.color;
            flashingColor.g = Mathf.Abs(Mathf.Sin(flashingAnimation));
            flashingColor.b = Mathf.Abs(Mathf.Sin(flashingAnimation));
            highlightSprite.color = flashingColor;
        }
    }

    void OnMouseDown() {
        // Go to alternate scene, loading this specified row
        RootMinigameManager.currentSlot = slotNumber;
        SceneManager.LoadScene("RootMinigame");
    }

    void OnMouseEnter() {
        highlightSprite.enabled = true;
        flashingAnimation = 1f;
    }

    void OnMouseExit() {
        highlightSprite.enabled = false;
    }
}
