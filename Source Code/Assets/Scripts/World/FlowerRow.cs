using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class FlowerRow : MonoBehaviour {
    public SpriteRenderer highlightSprite;
    public SpriteRenderer rowAvailableSprite;
    public int slotNumber;
    public GameObject flowerTestPrefab;
    [HideInInspector] public float flashingAnimation = 0f;

    [HideInInspector] public List<GameObject> overworldFlowerObjects;
    private bool finished = false;


    void Start() {
        highlightSprite.enabled = false;
        GenerateOverworldFlowers();

        if (PauseMenu.Instance != null) {
            finished = !PauseMenu.Instance.playerBoughtFirstTime;
            rowAvailableSprite.enabled = !finished;
        }
    }

    void Update() {
        if (rowAvailableSprite.enabled) {
            flashingAnimation += Time.deltaTime * 2f;

            // Plots of land with nutrients still available have a yellow glow that fades in and out
            Color rowAvailableColor = rowAvailableSprite.color;
            rowAvailableColor.a = Mathf.Abs(Mathf.Sin(flashingAnimation)) * 0.25f;
            rowAvailableSprite.color = rowAvailableColor;

            // Causes the highlight selector graphic to flash red so players have an easier time seeing it
            if (highlightSprite.enabled) {
                Color flashingColor = highlightSprite.color;
                flashingColor.g = Mathf.Abs(Mathf.Sin(flashingAnimation));
                flashingColor.b = Mathf.Abs(Mathf.Sin(flashingAnimation));
                highlightSprite.color = flashingColor;
            }
        }
    }

    void OnMouseDown() {
        if (finished || OverworldManager.Instance.fadeOutAnimation != 0f) return;

        if (PauseMenu.Instance) {
            // Prevent clicking if we are paused or game overed
            if (PauseMenu.Instance.menuCanvas.enabled) return;

            // When exiting the minigame, the player will be next to it on the pathway to their house
            PauseMenu.Instance.playerPosition = new Vector3(-0.715f, transform.position.y, 0f);
        }

        // Go to alternate scene, loading this specified row
        RootMinigameManager.currentSlot = slotNumber;

        OverworldManager.Instance.cinematicCameraTarget = transform;
        OverworldManager.Instance.TransitionToScene("RootMinigame");
    }

    void OnMouseEnter() {
        if (finished) return;
        highlightSprite.enabled = true;
    }

    void OnMouseExit() {
        if (finished) return;
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
                int[] colorIndex = data.colorIndex;

                for (int i = 0; i < coordinates.Length; i++) {
                    coordinates[i] = new Vector3(data.xCoordinates[i], 0f, 0f);
                }

                // Seeded randomization so its always consistent, if we decide to implement some variance
                Random.InitState(data.generationSeed);

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

                                // Set flower color and intensity
                                newOverworldFlower.GetComponent<OverworldFlower>().SetColorIntensity(colorIndex[r], nutrientCount[r] - 1);

                                // Randomly flip flower sprite to make it look a bit less repetitive
                                // Shader currently does not support two-sided so this just makes them invisible
                                // if (Random.value >= 0.5f) newOverworldFlower.GetComponent<SpriteRenderer>().flipX = true;

                                // Adds it to a list so we can clear it on command if we want to regenerate this (i.e. pick the flower on overworld)
                                overworldFlowerObjects.Add(newOverworldFlower);
                            }
                        }
                        runningIndex++;
                    }
                }

                // Check if there are any nutrients left. If not, mark this plot as finished
                finished = true;
                bool[] nutrientsAvailable = data.nutrientsEnabled;
                foreach(bool na in nutrientsAvailable) {
                    if (na == true) finished = false;
                }

                rowAvailableSprite.enabled = !finished;
            }

            stream.Close();
        }
    }
}
