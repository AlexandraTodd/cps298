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
    string targetSceneName = "";

    public AudioSource music;
    public AudioLowPassFilter lowPassFilter;
    public Camera mainCamera;
    public Transform cinematicCameraTarget;
    public Image fadeOutOverlay;
    public GameObject appraisalPrompt;
    public AudioSource soundEffects;
    public AudioClip sound_money;

    [HideInInspector] public bool writeNewPersonalBest = true;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        fadeOutOverlay.enabled = false;
        lowPassFilter.cutoffFrequency = 22000f;

        // Dramatically fade in
        mainCamera.orthographicSize = 0.5f;
        fadeInAnimation = 1f;

        CloseAppraisalPrompt();

        // Annoyingly enough, these need to be updated every scene change
        PauseMenu.UpdateAudioPreference();
    }

    // Update is called once per frame
    void Update() {
        // Fade in from white at the start
        if (fadeInAnimation > 0f) {
            // 1.551535 is the default set
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 1.551535f, Time.deltaTime * 4f);
            fadeOutOverlay.enabled = true;

            fadeInAnimation = Mathf.Max(0f, fadeInAnimation - Time.deltaTime);

            fadeOutOverlay.color = new Color(1f, 1f, 1f, fadeInAnimation);

            music.volume = 1f - fadeInAnimation;

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

            // Music fades out
            music.volume = Mathf.Min(1f, fadeOutAnimation);

            // Fades out 1 fadeOutAnimation per second, when reaching 0 it completes the switch to another scene
            fadeOutAnimation = Mathf.Max(0f, fadeOutAnimation - Time.deltaTime);
            if (fadeOutAnimation == 0f) {
                SceneManager.LoadScene(targetSceneName);
            }
        } else {
            if (PauseMenu.Instance != null) {
                lowPassFilter.cutoffFrequency = Mathf.Lerp(lowPassFilter.cutoffFrequency, (PauseMenu.Instance.menuCanvas.enabled ? 500f : 22000f), Time.deltaTime * 8f);
                music.volume = Mathf.Lerp(music.volume, (PauseMenu.Instance.menuCanvas.enabled ? 0.1f : 1f), Time.deltaTime * 4f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            OpenAppraisalPrompt();
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
        int numberOfFlowersPlanted = 0;
        int totalNutrientCount = 0;
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

                    distanceBetweenFlowers[flowerRow] = distanceBetweenFlowersThisRowTally / firstXPositionsOfFlowers.Count;
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

        // Truncate each being out of 5 stars with 2 decimal points (kept separate for readability and ease of changing formulas if needed)
        /*
        scoreVariance = Mathf.Clamp(Mathf.Round(scoreVariance * 500f) / 100f, 0f, 5f);
        scoreVolume = Mathf.Clamp(Mathf.Round(scoreVolume * 500f) / 100f, 0f, 5f);
        scoreVibrance = Mathf.Clamp(Mathf.Round(scoreVibrance * 500f) / 100f, 0f, 5f);
        scoreSpread = Mathf.Clamp(Mathf.Round(scoreSpread * 500f) / 100f, 0f, 5f);
        */

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

        soundEffects.PlayOneShot(sound_money);

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

        // Increment money goes here; needs to be saved before doing this reload

        // Reset scene
        CloseAppraisalPrompt();
        cinematicCameraTarget = mainCamera.transform;
        TransitionToScene("TownMap");
    }
}
