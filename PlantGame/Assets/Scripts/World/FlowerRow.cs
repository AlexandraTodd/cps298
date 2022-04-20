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

    [HideInInspector] public List<GameObject> overworldFlowerObjects;

    void Awake() {
        highlightSprite.enabled = false;
        GenerateOverworldFlowers();
    }

    void Update() {
        // Causes the highlight selector graphic to flash red so players have an easier time seeing it
        if (highlightSprite.enabled) {
            flashingAnimation += Time.deltaTime * 4f;
            Color flashingColor = highlightSprite.color;
            flashingColor.g = Mathf.Abs(Mathf.Sin(flashingAnimation));
            flashingColor.b = Mathf.Abs(Mathf.Sin(flashingAnimation));
            highlightSprite.color = flashingColor;
        }
    }

    void OnMouseDown() {
        // When exiting the minigame, the player will be next to it on the pathway to their house
        PauseMenu.Instance.playerPosition = new Vector3(-0.715f, transform.position.y, 0f); ;

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

    public void GenerateOverworldFlowers() {
        // If this needs to be refreshed, clear all overworld flowers created
        foreach (GameObject go in overworldFlowerObjects) Destroy(go);
        overworldFlowerObjects.Clear();

        string path = Application.persistentDataPath + "/roots" + slotNumber + ".dat";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length != 0) {
                RootMinigameSave data = (RootMinigameSave)(formatter.Deserialize(stream));

                // Reconstruct float coordinates into vector2s for linerenderers to read
                int[] pointsPerRoot = data.pointsPerLine;
                Vector3[] coordinates = new Vector3[data.xCoordinates.Length];
                int[] nutrientCount = data.nutrientCount;
                for (int i = 0; i < coordinates.Length; i++) {
                    coordinates[i] = new Vector3(data.xCoordinates[i], data.yCoordinates[i], 0f);
                }

                // There's probably a much more efficient way of doing this? I'm tired lol
                // Grabs the first point of each point per root in the coordinate list
                int runningIndex = 0;
                for (int r = 0; r < pointsPerRoot.Length; r++) {
                    for (int p = 0; p < pointsPerRoot[r]; p++) {
                        if (p == 0) {
                            if (nutrientCount[r] > 0) {
                                GameObject newOverworldFlower = Instantiate(flowerTestPrefab, transform);

                                // Extrapolate a location based on the x coordinate
                                // In the minigame, this ranges from -46 to 46
                                // In the overworld, this ranges from -0.23 to 0.23
                                newOverworldFlower.transform.localPosition = new Vector3((coordinates[runningIndex].x) * 0.005f, 0f, 0f);

                                // As a test; white appears for 1 nutrient flowers, gray appears for 2 nutrient flowers, and black appears for 3 nutrient flowers
                                float colorScale = 1f - (nutrientCount[r] / 3f);
                                newOverworldFlower.GetComponent<SpriteRenderer>().color = new Color(colorScale, colorScale, colorScale, 1f);
                                
                                // Adds it to a list so we can clear it on command if we want to regenerate this (i.e. pick the flower on overworld)
                                overworldFlowerObjects.Add(newOverworldFlower);
                            }
                        }
                        runningIndex++;
                    }
                }
            }
        }
    }
}