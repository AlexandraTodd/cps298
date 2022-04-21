using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System;

public class RootMinigameManager : MonoBehaviour {
    [HideInInspector] public static RootMinigameManager Instance;
    [HideInInspector] public List<PlantedRoot> plantedRoots = new List<PlantedRoot>();
    [HideInInspector] public List<GameObject> obstacles = new List<GameObject>();
    [HideInInspector] public List<GameObject> aquifers = new List<GameObject>();
    [HideInInspector] public List<GameObject> nutrients = new List<GameObject>();
    [HideInInspector] public bool[] nutrientsEnabled = new bool[12];
    [HideInInspector] public int generationSeed = -1;
    public GameObject plantedRootPrefab;
    public GameObject activeRootPrefab;
    public GameObject obstaclePrefab;
    public GameObject aquiferPrefab;
    public GameObject nutrientPrefab;
    public Camera defaultCamera;
    public GameObject spawnPointer;
    [HideInInspector] public float delayBeforeReturnToDefaultView = 0f;
    [HideInInspector] public ActiveRoot activeRoot;
    public static int currentSlot = -1;
    public bool paused = false;

    public GameObject instructionPrompt;
    public TMP_Text instructionPromptText;
    public GameObject exitMinigameButton;
    [HideInInspector] public float fadeInAnimation = 0f;
    [HideInInspector] public float fadeOutAnimation = 0f;
    public Image fadeAnimationOverlay;

    [HideInInspector] public int cursorGrabbed = -1;
    public Image cursorImage;

    public GameObject placeholderSeedList;
    public GameObject placeholderSeedPrefab;

    void Awake() {
        Instance = this;

        // Opening fade in animation
        cursorImage.enabled = false;
        exitMinigameButton.SetActive(false);
        placeholderSeedList.SetActive(false);
        fadeAnimationOverlay.enabled = true;
        fadeAnimationOverlay.color = Color.black;
        fadeInAnimation = 1f;
        
        // Create a seed for each of the 12 colors
        for (int i = 0; i < 12; i++) {
            GameObject newSeedDrag = Instantiate(placeholderSeedPrefab, placeholderSeedList.transform);
            newSeedDrag.GetComponent<PlaceholderSeedDrag>().SetColor(i);
        }

        instructionPromptText.text = "Guide the flowers' roots to the nutrients and water they need!\n\n� Use your mouse to click on and drag a seed from the menu on the left onto the planter."
            + "\n\n� A root will then emerge and follow your mouse. Guide it into one or more " + $"{"nutrients".AddColor(new Color(0.6f, 0.4f, 0f, 1f))}" + " before completing it by reaching a source of "
            + $"{"water".AddColor(new Color(0f, 0.75f, 1f, 1f))}" + "."
            + "\n\n� Avoid colliding with " + $"{"rocks".AddColor(Color.black)}" + " or other roots - this will break your root!"
            +"\n\n� Roots thin out the more they travel upward and eventually die.";

        LoadPlantMinigameProgress();
    }

    private IEnumerator PauseWait() {
        paused = true;
        yield return new WaitForSeconds(1);
        paused = false;
        activeRoot.GetComponent<Rigidbody2D>().WakeUp();
    }

    private IEnumerator ExitWait() {
        yield return new WaitForSeconds(1);
        InitiateExit();
    }

    public void InitiateExit() {
        fadeOutAnimation = 1f;
    }

    void Update() {
        if (fadeOutAnimation > 0f) {
            exitMinigameButton.SetActive(false);
            placeholderSeedList.SetActive(false);
            delayBeforeReturnToDefaultView = 1f;
            fadeAnimationOverlay.enabled = true;
            fadeAnimationOverlay.color = new Color(1f, 1f, 1f, 1f - fadeOutAnimation);

            defaultCamera.orthographicSize = Mathf.Lerp(defaultCamera.orthographicSize, 1000f, Time.deltaTime * 0.5f);

            fadeOutAnimation = Mathf.Max(0f, fadeOutAnimation - Time.deltaTime);
            if (fadeOutAnimation == 0f) {
                SceneManager.LoadScene("TownMap");
            }
        }

        if (fadeInAnimation > 0f) {
            fadeAnimationOverlay.enabled = true;
            Color fadeInOverlayColor = fadeAnimationOverlay.color;
            fadeInOverlayColor.a = Mathf.Min(1f, fadeInOverlayColor.a - (Time.deltaTime * 2f));
            fadeAnimationOverlay.color = fadeInOverlayColor;

            fadeInAnimation = Mathf.Max(0f, fadeInAnimation - Time.deltaTime);
            if (fadeInAnimation == 0f) {
                fadeAnimationOverlay.enabled = false;
                instructionPrompt.SetActive(true);
            }
        }

        // No active root
        if (activeRoot == null) {
            // postDeathDelay 
            delayBeforeReturnToDefaultView = Mathf.Max(0f, delayBeforeReturnToDefaultView - Time.deltaTime);
            if (delayBeforeReturnToDefaultView == 0f) {
                if (fadeInAnimation == 0f) {
                    exitMinigameButton.SetActive(true);
                    placeholderSeedList.SetActive(true);
                }

                defaultCamera.orthographicSize = Mathf.Lerp(defaultCamera.orthographicSize, 100f, Time.deltaTime * 4f);
                defaultCamera.transform.position = Vector3.Lerp(defaultCamera.transform.position, new Vector3(0f, -100f, -10f), Time.deltaTime * 4f);

                // Check if nutrients are available
                // If there are 0, then planting roots is useless and we should not allow players to do it
                bool nutrientsAvailable = false;
                for(int i = 0; i < nutrientsEnabled.Length; i++) {
                    if (nutrientsEnabled[i]) nutrientsAvailable = true;
                }

                if (nutrientsAvailable) {
                    // Grabs where our cursor is
                    Vector2 testPoint = defaultCamera.ScreenToWorldPoint(Input.mousePosition);

                    // If we're dragging a seed, drag it where our cursor is
                    if (cursorGrabbed != -1) {
                        if (!cursorImage.enabled) {
                            cursorImage.enabled = true;

                            // Transparent as a visual cue its not being dropped in a proper spot
                            Color cursorImageColor = cursorImage.color;
                            cursorImageColor.a = 0.25f;
                            cursorImage.color = cursorImageColor;
                        }
                        cursorImage.transform.position = testPoint;
                    } else if (cursorImage.enabled) cursorImage.enabled = false;

                    // If we're dragging a seed near the top of the planter, show a cursor, and release mouse to begin planting it
                    if (testPoint.y >= -90f && testPoint.x <= 46f && testPoint.x >= -46f && cursorGrabbed != -1) {
                        if (!spawnPointer.activeInHierarchy) spawnPointer.SetActive(true);
                        spawnPointer.transform.position = new Vector3(testPoint.x, -4f, 0f);

                        // Snap cursor image to right under the spawn arrow as a visual aid, and make its visual opaque
                        cursorImage.transform.position = new Vector3(testPoint.x, -12f, 0f);
                        Color cursorImageColor = cursorImage.color;
                        cursorImageColor.a = Mathf.Lerp(cursorImageColor.a, 1f, Time.deltaTime * 4f);
                        cursorImage.color = cursorImageColor;

                        // Let go of mouse while dragging a seed into the planter, initiates the minigame with the seed type we want
                        if (Input.GetMouseButtonUp(0)) {
                            // Creates the root
                            GameObject newRoot = Instantiate(activeRootPrefab, new Vector2(spawnPointer.transform.position.x, 0f), Quaternion.Euler(new Vector3(0f, 0f, 270f)));
                            exitMinigameButton.SetActive(false);
                            placeholderSeedList.SetActive(false);
                            activeRoot = newRoot.GetComponent<ActiveRoot>();
                            activeRoot.colorIndex = cursorGrabbed;
                            activeRoot.manager = this;
                            spawnPointer.SetActive(false);

                            cursorImage.enabled = false;
                            cursorGrabbed = -1;

                            StartCoroutine(PauseWait());
                        }
                    } else {
                        // Our mouse is currently not in the planter area, so hide the arrow helping us decide where to spawn
                        if (spawnPointer.activeInHierarchy) spawnPointer.SetActive(false);
                        if (cursorGrabbed != -1) {
                            if (Input.GetMouseButtonUp(0)) {
                                cursorGrabbed = -1;
                                cursorImage.enabled = false;
                            }
                        }
                    }
                } else {
                    // Game automatically goes back to overworld if we're out of nutrients (thanks Jack!)
                    if (spawnPointer.activeInHierarchy) spawnPointer.SetActive(false);
                    StartCoroutine(ExitWait());
                }
            }
        } else {
            delayBeforeReturnToDefaultView = 1f;
            defaultCamera.orthographicSize = Mathf.Lerp(defaultCamera.orthographicSize, 10f, Time.deltaTime * 16f);
            Vector3 activeRootPosition = activeRoot.transform.position;
            defaultCamera.transform.position = Vector3.Lerp(defaultCamera.transform.position, new Vector3(activeRootPosition.x, activeRootPosition.y, -10f), Time.deltaTime * 16f);

            if (!paused) {
                activeRoot.target = defaultCamera.ScreenToWorldPoint(Input.mousePosition);

                if (activeRoot.currentlyRotating) {
                    // Because we are changing angles, add an extra point to our linerenderer 10 times a second
                    // This would be for saving roots and minimizing linerender calculations for performance purposes
                    // The longer the timer, the more rigid the line appears but the healthier performance will be
                    activeRoot.generatePointTimer = Mathf.Max(0f, activeRoot.generatePointTimer - Time.deltaTime);
                    if (activeRoot.generatePointTimer == 0f) {
                        activeRoot.stem.positionCount++;
                        activeRoot.generatePointTimer = 0.1f;
                    }
                } else activeRoot.generatePointTimer = 0f;
            }
        }

        // Exit
        if (Input.GetKeyDown(KeyCode.Escape)) {
            InitiateExit();
        }
    }

    void LoadPlantMinigameProgress() {
        // Variables needed to reconstruct an in-progress plant minigame
        // If a save isn't found, this will create a brand new setup
        generationSeed = UnityEngine.Random.Range(0, 2147483647);
        Vector3[] coordinates = new Vector3[0];
        int[] pointsPerRoot = new int[0];
        int[] rootStartingColor = new int[0];
        int[] rootEndingColor = new int[0];
        float[] rootEndWidth = new float[0];
        int[] rootColorIndex = new int[0];
        int[] rootNutrientCount = new int[0];

        // Enable all nutrients by default if its a new map
        nutrientsEnabled = new bool[RandomNutrientCount()];
        for (int i = 0; i < nutrientsEnabled.Length; i++) nutrientsEnabled[i] = true;

        // Load save and replace generation parameters if available
        string path = Application.persistentDataPath + "/roots"+currentSlot+".dat";
        if (File.Exists(path)) {
            Debug.Log("File found, loading");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length != 0) {
                RootMinigameSave data = (RootMinigameSave)(formatter.Deserialize(stream));

                // Reconstruct float coordinates into vector2s for linerenderers to read
                pointsPerRoot = data.pointsPerLine;
                coordinates = new Vector3[data.xCoordinates.Length];
                for (int i = 0; i < coordinates.Length; i++) {
                    coordinates[i] = new Vector3(data.xCoordinates[i], data.yCoordinates[i], 0f);
                }

                // Inherit miscellaneous saved properties
                rootStartingColor = data.startingColor;
                rootEndingColor = data.endingColor;
                rootEndWidth = data.endWidth;
                rootNutrientCount = data.nutrientCount;
                rootColorIndex = data.colorIndex;
                generationSeed = data.generationSeed;
                nutrientsEnabled = data.nutrientsEnabled;
            }

            stream.Close();
        }

        // Rebuild planted roots from stored coordinates
        int runningIndex = 0;
        for (int r = 0; r < pointsPerRoot.Length; r++) {
            GameObject plantedRoot = Instantiate(plantedRootPrefab, transform.position, Quaternion.identity);
            PlantedRoot plantedRootDetail = plantedRoot.GetComponent<PlantedRoot>();
            plantedRootDetail.stem.positionCount = pointsPerRoot[r];

            // Separate components of color
            // First two digits are alpha, next two R, next two G, next two B. Convert from int to decimal float
            string startingColorConstructor = rootStartingColor[r].ToString();
            float startingColorA = (Int32.Parse(startingColorConstructor.Substring(0, 2)) + 1) / 100f;
            float startingColorR = (Int32.Parse(startingColorConstructor.Substring(2, 2)) + 1) / 100f;
            float startingColorG = (Int32.Parse(startingColorConstructor.Substring(4, 2)) + 1) / 100f;
            float startingColorB = (Int32.Parse(startingColorConstructor.Substring(6, 2)) + 1) / 100f;
            plantedRootDetail.stem.startColor = new Color(startingColorR, startingColorG, startingColorB, startingColorA);

            string endingColorConstructor = rootEndingColor[r].ToString();
            float endingColorA = (Int32.Parse(endingColorConstructor.Substring(0, 2)) + 1) / 100f;
            float endingColorR = (Int32.Parse(endingColorConstructor.Substring(2, 2)) + 1) / 100f;
            float endingColorG = (Int32.Parse(endingColorConstructor.Substring(4, 2)) + 1) / 100f;
            float endingColorB = (Int32.Parse(endingColorConstructor.Substring(6, 2)) + 1) / 100f;
            plantedRootDetail.stem.endColor = new Color(endingColorR, endingColorG, endingColorB, endingColorA);

            plantedRootDetail.stem.endWidth = rootEndWidth[r];
            plantedRootDetail.colorIndex = rootColorIndex[r];
            plantedRootDetail.nutrientCount = rootNutrientCount[r];

            for (int p = 0; p < pointsPerRoot[r]; p++) {
                plantedRootDetail.stem.SetPosition(p, coordinates[runningIndex]);
                runningIndex++;
            }

            plantedRoots.Add(plantedRootDetail);
        }

        // This fixes the random generation to the loaded saves, causing it to produce the exact same results
        UnityEngine.Random.InitState(generationSeed);

        // Generate aquifers
        int numberOfAquifers = UnityEngine.Random.Range(1, 4);
        for (int i = 0; i < numberOfAquifers; i++) {
            Vector3 newPos = new Vector3(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(-120f, -150f), 0f);
            GameObject newAquifer = Instantiate(aquiferPrefab, newPos, Quaternion.identity);
            float randomSizeModifier = (24f + (12f * UnityEngine.Random.value)) / numberOfAquifers;
            newAquifer.transform.localScale = new Vector3(randomSizeModifier, randomSizeModifier, randomSizeModifier);
            aquifers.Add(newAquifer);
        }

        // Granularity: the more granular, there are more but smaller obstacles
        float granularity = Mathf.Max(UnityEngine.Random.value, 0.2f);

        // Generate obstacles
        int numberOfObstacles = Mathf.RoundToInt(60f / granularity);
        for (int i = 0; i < numberOfObstacles; i++) {
            // This while loop is used to prevent obstacles from spawning too close to each other
            bool spotFound = false;
            while (!spotFound) {
                // Potential X coordinates narrow the farther down the level you go
                float newYCoordinate = UnityEngine.Random.Range(-4f, -150f);
                float newXCoordinate = 48f + (30f * (newYCoordinate / 150f));
                Vector3 newPos = new Vector3(UnityEngine.Random.Range(-newXCoordinate, newXCoordinate), newYCoordinate, 0f);

                spotFound = true;
                foreach (GameObject go in obstacles) {
                    if (Vector3.Distance(go.transform.position, newPos) <= 8f * granularity) spotFound = false;
                }

                if (spotFound) {
                    float randomSizeModifier = (16f * granularity) * Mathf.Max(UnityEngine.Random.value, 0.2f);
                    GameObject newObstacle = Instantiate(obstaclePrefab, newPos, Quaternion.identity);
                    newObstacle.transform.localScale = new Vector3(randomSizeModifier, randomSizeModifier, randomSizeModifier);

                    obstacles.Add(newObstacle);
                }
            }
        }

        // Generate nutrients
        for (int i = 0; i < nutrientsEnabled.Length; i++) {
            // This while loop is used to prevent obstacles from spawning too close to each other
            bool spotFound = false;
            while (!spotFound) {
                // Potential X coordinates narrow the farther down the level you go
                float newYCoordinate = UnityEngine.Random.Range(-9f, -140f);
                float newXCoordinate = 46f + (40f * (newYCoordinate / 150f));
                Vector3 newPos = new Vector3(UnityEngine.Random.Range(-newXCoordinate, newXCoordinate), newYCoordinate, 0f);

                spotFound = true;
                foreach (GameObject go in obstacles) {
                    if (Vector3.Distance(go.transform.position, newPos) <= 8f * granularity) spotFound = false;
                }

                foreach (GameObject go in aquifers) {
                    if (Vector3.Distance(go.transform.position, newPos) <= 32f) spotFound = false;
                }

                foreach (GameObject go in nutrients) {
                    if (Vector3.Distance(go.transform.position, newPos) <= 3f) spotFound = false;
                }

                if (spotFound) {
                    if (nutrientsEnabled[i]) {
                        GameObject newNutrient = Instantiate(nutrientPrefab, newPos, Quaternion.identity);
                        newNutrient.GetComponent<Nutrient>().index = i;
                        nutrients.Add(newNutrient);
                    }
                }
            }
        }
    }

    public void SaveRoots() {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/roots"+currentSlot+".dat";
        FileStream stream = new FileStream(path, FileMode.Create);

        List<Vector3> outputCoordinates = new List<Vector3>();
        List<int> outputPointsPerRoot = new List<int>();
        List<int> outputStartingColors = new List<int>();
        List<int> outputEndingColors = new List<int>();
        List<float> outputEndWidth = new List<float>();
        List<int> outputColorIndex = new List<int>();
        List<int> outputNutrientCount = new List<int>();

        for (int i = 0; i < plantedRoots.Count; i++) {
            outputPointsPerRoot.Add(plantedRoots[i].stem.positionCount);

            // GetPositions requires an output variable, I believe
            Vector3[] output = new Vector3[plantedRoots[i].stem.positionCount];
            plantedRoots[i].stem.GetPositions(output);

            for (int p = 0; p < output.Length; p++) {
                outputCoordinates.Add(output[p]);
            }

            // Start color converted to a coded int for easy storage
            // Alpha | R | G | B
            string constructionStartOutput;
            constructionStartOutput = Mathf.Max(10f, Mathf.Round((plantedRoots[i].stem.startColor.a * 100f) - 1f)).ToString();
            constructionStartOutput += Mathf.Max(10f, Mathf.Round((plantedRoots[i].stem.startColor.r * 100f) - 1f)).ToString();
            constructionStartOutput += Mathf.Max(10f, Mathf.Round((plantedRoots[i].stem.startColor.g * 100f) - 1f)).ToString();
            constructionStartOutput += Mathf.Max(10f, Mathf.Round((plantedRoots[i].stem.startColor.b * 100f) - 1f)).ToString();
            outputStartingColors.Add(Int32.Parse(constructionStartOutput));

            // End color converted to a coded int for easy storage
            // Alpha | R | G | B
            string constructionEndOutput;
            constructionEndOutput = Mathf.Max(10f, Mathf.Round((plantedRoots[i].stem.endColor.a * 100f) - 1f)).ToString();
            constructionEndOutput += Mathf.Max(10f, Mathf.Round((plantedRoots[i].stem.endColor.r * 100f) - 1f)).ToString();
            constructionEndOutput += Mathf.Max(10f, Mathf.Round((plantedRoots[i].stem.endColor.g * 100f) - 1f)).ToString();
            constructionEndOutput += Mathf.Max(10f, Mathf.Round((plantedRoots[i].stem.endColor.b * 100f) - 1f)).ToString();
            outputEndingColors.Add(Int32.Parse(constructionEndOutput));

            outputEndWidth.Add(plantedRoots[i].stem.endWidth);
            outputColorIndex.Add(plantedRoots[i].colorIndex);
            outputNutrientCount.Add(plantedRoots[i].nutrientCount);
        }

        RootMinigameSave data = new RootMinigameSave(outputCoordinates.ToArray(), outputPointsPerRoot.ToArray(), generationSeed, 
            outputStartingColors.ToArray(), outputEndingColors.ToArray(), outputEndWidth.ToArray(), nutrientsEnabled, outputColorIndex.ToArray(), outputNutrientCount.ToArray());
        formatter.Serialize(stream, data);
        stream.Close();
    }

    // This is called several times, put into a separate function to help ensure its consistent
    public int RandomNutrientCount() {
        return 7 + UnityEngine.Random.Range(0, 7);
    }

    public void CloseInstructionPrompt() {
        instructionPrompt.SetActive(false);
    }
}
