using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        fadeOutOverlay.enabled = false;
        lowPassFilter.cutoffFrequency = 22000f;

        // Dramatically fade in
        mainCamera.orthographicSize = 0.5f;
        fadeInAnimation = 1f;
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

            music.volume = 0.5f * (1f - fadeInAnimation);

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
            music.volume = 0.5f * fadeOutAnimation;

            // Fades out 1 fadeOutAnimation per second, when reaching 0 it completes the switch to another scene
            fadeOutAnimation = Mathf.Max(0f, fadeOutAnimation - Time.deltaTime);
            if (fadeOutAnimation == 0f) {
                SceneManager.LoadScene(targetSceneName);
            }
        } else {
            if (PauseMenu.Instance != null) lowPassFilter.cutoffFrequency = Mathf.Lerp(lowPassFilter.cutoffFrequency, (PauseMenu.Instance.menuCanvas.enabled ? 500f : 22000f), Time.deltaTime * 8f);
            if (PauseMenu.Instance != null) music.volume = Mathf.Lerp(music.volume, (PauseMenu.Instance.menuCanvas.enabled ? 0.1f : 0.5f), Time.deltaTime * 4f);
        }
    }

    public void TransitionToScene(string s) {
        fadeOutAnimation = 1.5f;
        fadeOutOverlay.color = new Color(1f, 1f, 1f, 0f);
        targetSceneName = s;
    }
}
