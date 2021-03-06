using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class OverworldManager : MonoBehaviour {
    [HideInInspector] public static OverworldManager Instance;
    [HideInInspector] public float fadeOutAnimation = 0f;
    [HideInInspector] public float fadeInAnimation = 0f;
    [HideInInspector] public bool controllingCamera = false;
    [HideInInspector] public bool writeNewPersonalBest = true;
    string targetSceneName = "";

    public Camera mainCamera;
    public Transform cinematicCameraTarget;
    public Image fadeOutOverlay;
    public GameObject appraisalPrompt;
    public AudioSource soundEffects;
    public AudioClip sound_money;

    public GameObject shopInstructions;
    public GameObject goBuyInstructions;
    public GameObject goPlantInstructions;
    public GameObject goSellInstructions;
    public GameObject appraisalInstructions;

    [HideInInspector] public bool noMoney = false;
    [HideInInspector] public bool noFlowersPlanted = true;
    [HideInInspector] public bool noItemsInInventory = true;

    // Start is called before the first frame update
    void Start() {
        Instance = this;
        fadeOutOverlay.enabled = false;

        // Dramatically fade in
        mainCamera.orthographicSize = 0.5f;
        fadeInAnimation = 1f;

        CloseAppraisalPrompt();

        if (PauseMenu.Instance != null) {
            // Switch to overworld music if we're playing minigame music (this keeps music between overworld and shop despite being separate scenes)
            if (!PauseMenu.MusicIsPlayingClip(PauseMenu.Instance.townmusic)) PauseMenu.SetMusic(PauseMenu.Instance.townmusic);

            // Show instructions by circumstance
            if (PauseMenu.Instance.showShopInstructions) {
                shopInstructions.SetActive(true);
                goBuyInstructions.SetActive(true);
                appraisalInstructions.SetActive(false);
                goSellInstructions.SetActive(false);
                goPlantInstructions.SetActive(false);
            } else {
                shopInstructions.SetActive(false);
                goBuyInstructions.SetActive(false);

                goPlantInstructions.SetActive(PauseMenu.Instance.showMinigameInstructions);

                appraisalInstructions.SetActive(PauseMenu.Instance.showAppraisalInstructions);
                goSellInstructions.SetActive(PauseMenu.Instance.showAppraisalInstructions);
            }
        }

        // Annoyingly enough, these need to be updated every scene change
        PauseMenu.UpdateAudioPreference();

        // Check if we have any items to sell or not
        string path = Application.persistentDataPath + "/inventory.dat";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length != 0) {
                // This loads in a binary file and reconstructs its integer arrays into List items
                InventorySave data = (InventorySave)(formatter.Deserialize(stream));
                int[] itemListType = data.itemListType;
                int[] itemListStack = data.itemListStack;
                int[] itemListColor = data.itemListColor;
                int[] itemListIntensity = data.itemListIntensity;

                for (int i = 0; i < itemListType.Length; i++) {
                    if (itemListType[i] == 1 && itemListStack[i] > 0) {
                        noItemsInInventory = false;
                    }
                }
            }

            stream.Close();
        }

        // Check if we have flowers or not
        for (int i = 0; i < 10; i++) {
            path = Application.persistentDataPath + "/roots" + i + ".dat";
            if (File.Exists(path)) {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                if (stream.Length != 0) {
                    RootMinigameSave data = (RootMinigameSave)(formatter.Deserialize(stream));
                    int[] nutrientCount = data.nutrientCount;
                    for (int p = 0; p < nutrientCount.Length; p++) {
                        if (nutrientCount[p] > 0) noFlowersPlanted = false;
                    }
                }

                stream.Close();
            }
        }
    }

    // Update is called once per frame
    void Update() {
        // Fade in from white at the start
        if (fadeInAnimation > 0f) {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 1f, Time.deltaTime * 4f);
            fadeOutOverlay.enabled = true;
            fadeInAnimation = Mathf.Max(0f, fadeInAnimation - Time.deltaTime);
            fadeOutOverlay.color = new Color(1f, 1f, 1f, fadeInAnimation);

            if (fadeInAnimation == 0f) {
                fadeOutOverlay.enabled = false;
            }
        }

        if (fadeOutAnimation > 0f) {
            // Takes control of the camera to make it pan towards the thing selected, rapidly zooms in
            controllingCamera = true;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cinematicCameraTarget.position, Time.deltaTime * 4f);
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 0.5f, Time.deltaTime * 4f);

            // Screen fades to white, which then drops to black as it moves to next scene
            fadeOutOverlay.enabled = true;
            Color fadeOutOverlayColor = fadeOutOverlay.color;

            if (fadeOutOverlayColor.a < 1f) fadeOutOverlayColor.a = Mathf.Min(1f, fadeOutOverlayColor.a + (Time.deltaTime * 2f));
            else {
                float fadeToBlack = Mathf.Max(0f, fadeOutOverlayColor.r - Time.deltaTime);
                fadeOutOverlayColor.r = fadeToBlack;
                fadeOutOverlayColor.g = fadeToBlack;
                fadeOutOverlayColor.b = fadeToBlack;
            }

            fadeOutOverlay.color = fadeOutOverlayColor;

            // Music fades out if its for the minigame
            if (targetSceneName == "RootMinigame") PauseMenu.SetMusicInGameVolume(Mathf.Min(1f, fadeOutAnimation));

            // Fades out 1 fadeOutAnimation per second, when reaching 0 it completes the switch to another scene
            fadeOutAnimation = Mathf.Max(0f, fadeOutAnimation - Time.deltaTime);
            if (fadeOutAnimation == 0f) {
                SceneManager.LoadScene(targetSceneName);
            }
        } else {
            if (PauseMenu.Instance != null) {
                PauseMenu.Instance.lowPassFilter.cutoffFrequency = Mathf.Lerp(PauseMenu.Instance.lowPassFilter.cutoffFrequency, (PauseMenu.Instance.menuCanvas.enabled ? 500f : 22000f), Time.deltaTime * 8f);
                PauseMenu.Instance.music.volume = Mathf.Lerp(PauseMenu.Instance.music.volume, (PauseMenu.Instance.menuCanvas.enabled ? 0.1f : 1f), Time.deltaTime * 4f);
            }
        }
    }
    
    // LateUpdate calls every frame after Update. Wanted a frame of leeway in case of false game overs being triggered but might not be necessary
    void LateUpdate() {
        if(noMoney && noFlowersPlanted && noItemsInInventory) {
            if (PauseMenu.Instance != null && PauseMenu.Instance.gameOverTimer == 0f) PauseMenu.Instance.gameOverTimer = 5f;
        }
    }

    public void TransitionToScene(string s) {
        fadeOutAnimation = 1.5f;
        fadeOutOverlay.color = new Color(1f, 1f, 1f, 0f);
        targetSceneName = s;
    }

    public float[] GardenAppraisalValues() {
        // Things to keep track of as we cycle through each flower row
        float scoreVariance, scoreVolume, scoreVibrance, scoreSpread;
        bool[,] colorVariantsUsed = new bool[12,3];
        float numberOfFlowersPlanted = 0f;
        float totalNutrientCount = 0f;
        float[] distanceBetweenFlowers = new float[10];

        // Load in each flower row
        // Appraisal stats should be calculated so that no file being found does not error and instead returns a 0% or whatever
        for (int flowerRow = 0; flowerRow < 10; flowerRow++) {
            string path = Application.persistentDataPath + "/roots" + flowerRow + ".dat";
            if (File.Exists(path)) {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                if (stream.Length != 0) {
                    RootMinigameSave data = (RootMinigameSave)(formatter.Deserialize(stream));

                    // Reconstruct properties of successful flowers
                    int[] pointsPerRoot = data.pointsPerLine;
                    Vector3[] coordinates = new Vector3[data.xCoordinates.Length];
                    for (int i = 0; i < coordinates.Length; i++) {
                        coordinates[i] = new Vector3(data.xCoordinates[i], 0f, 0f);
                    }

                    int[] nutrientCount = data.nutrientCount;

                    List<float> firstXPositionsOfFlowers = new List<float>();
                    int runningIndex = 0;
                    for (int r = 0; r < pointsPerRoot.Length; r++) {
                        for (int p = 0; p < pointsPerRoot[r]; p++) {
                            if (p == 0) {
                                if (nutrientCount[r] > 0) {
                                    firstXPositionsOfFlowers.Add(coordinates[runningIndex].x);
                                    totalNutrientCount += nutrientCount[r];
                                    colorVariantsUsed[data.colorIndex[r], nutrientCount[r] - 1] = true;
                                }
                            }

                            runningIndex++;
                        }
                    }

                    // Remember that we can't just use pointsPerRoot.Length because we need to only measure successful roots, not all roots
                    numberOfFlowersPlanted += firstXPositionsOfFlowers.Count;

                    // For calculating distance between flowers, we need to sort them from left to right
                    // Otherwise, distance will be measured between flowers in order they were planted, which is significantly less intuitive
                    firstXPositionsOfFlowers.Sort();

                    // Tally total distance between successive flowers in a row
                    List<float> distancesBetweenFlowersThisRow = new List<float>();
                    for (int i = 0; i < firstXPositionsOfFlowers.Count; i++) {
                        bool hasNext = !(i == firstXPositionsOfFlowers.Count - 1);
                        if (hasNext) distancesBetweenFlowersThisRow.Add(Mathf.Abs(firstXPositionsOfFlowers[i] - firstXPositionsOfFlowers[i + 1]));
                    }

                    // Take average of distances between flowers and store it for scoring
                    float distanceBetweenFlowersThisRowTally = 0f;
                    for (int i = 0; i < distancesBetweenFlowersThisRow.Count; i++) {
                        distanceBetweenFlowersThisRowTally += distancesBetweenFlowersThisRow[i];
                    }

                    distanceBetweenFlowers[flowerRow] = distanceBetweenFlowersThisRowTally / Mathf.Max(1f, firstXPositionsOfFlowers.Count);
                }

                stream.Close();
            } 
        }

        // Tallying number of combinations used for Variance
        int numberOfColorIntensityCombinationsUsed = 0;
        for (int i = 0; i < 12; i++) {
            for (int j = 0; j < 3; j++) {
                if (colorVariantsUsed[i, j]) numberOfColorIntensityCombinationsUsed++;
            }
        }

        // Tally total distance between flowers
        float sumOfAverages = 0f;
        for (int i = 0; i < 10; i++) sumOfAverages += distanceBetweenFlowers[i];

        // Formulas for scoring
        // NOTE: Vibrance currently has a linear reward for higher tier flowers. Do we want to encourage higher value ones by giving them greater priority?
        //       (i.e. tier 3 is worth 5 instead of 3)
        scoreVariance = numberOfColorIntensityCombinationsUsed / 36f;   // Target: All 36 color combinations
        scoreVolume = (numberOfFlowersPlanted / 10f) / 5f;              // Target: 5 flowers per row
        scoreVibrance = (totalNutrientCount / Mathf.Max(1f, numberOfFlowersPlanted)) / 3f; // Target: 3:1 nutrient:flower ratio
        scoreSpread = (sumOfAverages / 10f) / 20f;                      // Target: 20 units of distance

        float finalScore = (scoreVariance + scoreVolume + scoreVibrance + scoreSpread) / 4f;
        finalScore = Mathf.Round(finalScore * 500f) / 100f;

        // Returns array of results
        return new float[] { scoreVariance, scoreVolume, scoreVibrance, scoreSpread, finalScore };
    }

    public void OpenAppraisalPrompt() {
        appraisalPrompt.SetActive(true);

        float[] appraisalValues = GardenAppraisalValues();

        // Figure out if this is a PB
        bool pbExists = false;
        string path = Application.persistentDataPath + "/personalbest.dat";
        float incomingScore = appraisalValues[4];
        float previousBestScore = 0f;
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length != 0) {
                pbExists = true;
                PersonalBestSave pb = (PersonalBestSave)(formatter.Deserialize(stream));
                previousBestScore = pb.previousBest;
                writeNewPersonalBest = incomingScore > previousBestScore;
            }

            stream.Close();
        } else writeNewPersonalBest = true;

        // Update values
        appraisalPrompt.GetComponent<AppraisalPrompt>().UpdateValues(appraisalValues, pbExists, writeNewPersonalBest, previousBestScore);
    }

    public void CloseAppraisalPrompt() {
        appraisalPrompt.SetActive(false);
    }

    public void AppraisalHarvestAndSell() {
        float incomingScore = GardenAppraisalValues()[4];

        // Handling currency
        if (incomingScore > 0f) {
            // Check if we already have a currency value saved to increment
            // If not, it will use the default 25
            int outputCurrency = 25;
            string path = Application.persistentDataPath + "/currency.dat";
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream;
            if (File.Exists(path)) {
                stream = new FileStream(path, FileMode.Open);
                if (stream.Length != 0) {
                    CurrencySave data = (CurrencySave)(formatter.Deserialize(stream));
                    outputCurrency = data.currency;
                }

                stream.Close();
            }

            // Increment new currency
            outputCurrency += Mathf.RoundToInt(incomingScore * 20f);

            // Save new currency value
            stream = new FileStream(path, FileMode.Create);
            CurrencySave newPB = new CurrencySave(outputCurrency);
            formatter.Serialize(stream, newPB);
            stream.Close();

            // Ka-ching
            soundEffects.PlayOneShot(sound_money);
            if (PauseMenu.Instance != null) PauseMenu.Instance.showAppraisalInstructions = false;
        }

        // Writes a new personal best if necessary. Made a separate file so it works between different playthroughs
        if (writeNewPersonalBest) {
            string path = Application.persistentDataPath + "/personalbest.dat";
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            PersonalBestSave newPB = new PersonalBestSave(incomingScore);
            formatter.Serialize(stream, newPB);
            stream.Close();
        }

        // Destroy all flower row files if they exist so we can create new plots
        for (int i = 0; i < 10; i++) {
            string path = Application.persistentDataPath + "/roots" + i + ".dat";
            if (File.Exists(path)) {
                File.Delete(path);
            }
        }

        // Reset scene
        CloseAppraisalPrompt();
        cinematicCameraTarget = mainCamera.transform;
        TransitionToScene("TownMap");
    }
}
