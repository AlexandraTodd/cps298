using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour {
    [HideInInspector] public static PauseMenu Instance;
    public Canvas menuCanvas;

    public Image blackTintBackground;
    public Button unpauseButton;
    public Button continueGameButton;
    public Button newGameButton;
    public Button settingsButton;
    public Button exitToMenuButton;
    public Button deletePersonalBestButton;

    [HideInInspector] public Vector3 playerPosition;

    public GameObject buttonsGroup;
    public GameObject settingsMenu;

    public AudioMixer soundAudioMixer;
    public TMP_Text soundVolumeLabel;
    public Slider soundVolumeSlider;

    public AudioMixer musicAudioMixer;
    public TMP_Text musicVolumeLabel;
    public Slider musicVolumeSlider;

    Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    public AudioSource music;
    public AudioLowPassFilter lowPassFilter;
    public AudioClip townmusic;
    public AudioClip minigamemusic;
    public AudioClip mainmenumusic;

    public TMP_Text titleText;
    public TMP_Text devCreditsText;
    public TMP_Text assetCreditsText;

    public GameObject gameOverScreen;

    [HideInInspector] public float gameOverTimer = 0f;

    [HideInInspector] public bool showMinigameInstructions = false;
    [HideInInspector] public bool showShopInstructions = false;
    [HideInInspector] public bool showAppraisalInstructions = false;
    [HideInInspector] public bool playerBoughtFirstTime = true;

    private void Awake() {
        // Singleton, important for returning to main menu from mid-game
        if (Instance != null) {
            Destroy(this.gameObject);
        } else Instance = this;

        DontDestroyOnLoad(Instance);
        ConfigureButtons(false);

        // Configure settings menu to display defaults
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++) {
            string option = resolutions[i].width + " x " + resolutions[i].height + " (" + resolutions[i].refreshRate + " Hz)";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && resolutions[i].refreshRate == Screen.currentResolution.refreshRate) {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        soundVolumeSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

        fullscreenToggle.isOn = PlayerPrefs.GetInt("UseFullScreen", 1) == 1;

        // Now actually set defaults using these displays to ensure parity
        SetSoundVolume(soundVolumeSlider.value);
        SetMusicVolume(musicVolumeSlider.value);
        SetResolution(currentResolutionIndex);
        SetFullScreen(fullscreenToggle.isOn);

        ToggleSettingsMenu(false);

        lowPassFilter.cutoffFrequency = 22000f;
    }

    public void Update() {
        if (gameOverTimer > 0f) {
            menuCanvas.enabled = true;
            gameOverScreen.SetActive(true);
            gameOverTimer = Mathf.Max(0f, gameOverTimer - Time.deltaTime);
            music.pitch = Mathf.Lerp(music.pitch, 0.5f, Time.deltaTime);
            buttonsGroup.SetActive(false);
            titleText.text = "";
            if (gameOverTimer == 0f) {
                gameOverScreen.SetActive(false);
                music.pitch = 1f;
                buttonsGroup.SetActive(true); ;
                DeleteSaveFiles();
                ExitToMenu();
            }
        } else {
            // Escape closes out of settings menu and, if in-game, pause menu
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (settingsMenu.activeInHierarchy) ToggleSettingsMenu(false);
                if (SceneManager.GetActiveScene().name == "TownMap") TogglePauseMenu(!menuCanvas.enabled);
            }
        }
    }

    public void TogglePauseMenu(bool enabled) {
        menuCanvas.enabled = enabled;
    }

    public void DeleteSaveFiles() {
        // Root slots
        for (int i = 0; i < 10; i++) {
            string path = Application.persistentDataPath + "/roots" + i + ".dat";
            if (File.Exists(path)) {
                File.Delete(path);
            }
        }

        // Town tile map
        string fieldPath = Application.persistentDataPath + "/field.dat";
        if (File.Exists(fieldPath)) {
            File.Delete(fieldPath);
        }

        // Inventory
        string inventoryPath = Application.persistentDataPath + "/inventory.dat";
        if (File.Exists(inventoryPath)) {
            File.Delete(inventoryPath);
        }

        // Currency
        string currencyPath = Application.persistentDataPath + "/currency.dat";
        if (File.Exists(currencyPath)) {
            File.Delete(currencyPath);
        }
    }

    public void NewGame() {
        // Check for save files and delete
        DeleteSaveFiles();

        // First time instructions will show up
        showShopInstructions = true;
        showAppraisalInstructions = false;
        showMinigameInstructions = true;
        playerBoughtFirstTime = false;

        // Calls the continue game function now that we have no save
        ContinueGame();
    }

    public void ContinueGame() {
        ConfigureButtons(true);
        TogglePauseMenu(false);

        SceneManager.LoadScene("TownMap");
    }

    public void ExitToMenu() {
        ConfigureButtons(false);
        TogglePauseMenu(true);
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void ConfigureButtons(bool isInGame) {
        blackTintBackground.enabled = isInGame;
        unpauseButton.gameObject.SetActive(isInGame);
        continueGameButton.gameObject.SetActive(!isInGame);
        newGameButton.gameObject.SetActive(!isInGame);
        exitToMenuButton.gameObject.SetActive(isInGame);

        // Resets player position to their house
        if (isInGame) {
            titleText.text = "Paused";
            playerPosition = new Vector3(-0.715f, 1.9f, 0f);
            devCreditsText.enabled = false;
            assetCreditsText.enabled = false;
        } else {
            titleText.text = "Back to the Roots";
            SetMusic(mainmenumusic);
            devCreditsText.enabled = true;
            assetCreditsText.enabled = true;
        }

        // Detect if we have a save file for continue game
        if (continueGameButton.gameObject.activeInHierarchy) {
            bool fileAvailable = false;

            string path;
            path = Application.persistentDataPath + "/currency.dat";
            if (File.Exists(path)) {
                fileAvailable = true;
            } else {
                path = Application.persistentDataPath + "/inventory.dat";
                if (File.Exists(path)) {
                    fileAvailable = true;
                } else {
                    // Check roots last to save 10 files worth of searching
                    for (int i = 0; i < 10; i++) {
                        path = Application.persistentDataPath + "/roots" + i + ".dat";
                        if (File.Exists(path)) {
                            fileAvailable = true;
                        }
                    }
                }
            }

            // If there's no files indicating we currently have a run, disable it
            continueGameButton.gameObject.SetActive(fileAvailable);
        }
    }

    public void ToggleSettingsMenu(bool b) {
        settingsMenu.SetActive(b);
        buttonsGroup.gameObject.SetActive(!b);
        if (SceneManager.GetActiveScene().name != "TownMap") blackTintBackground.enabled = b;

        // Check if we have a personal best
        string path = Application.persistentDataPath + "/personalbest.dat";
        if (File.Exists(path)) {
            deletePersonalBestButton.gameObject.SetActive(true);
        } else {
            deletePersonalBestButton.gameObject.SetActive(false);
        }
    }

    public void DeletePersonalBest() {
        File.Delete(Application.persistentDataPath + "/personalbest.dat");
        
        deletePersonalBestButton.gameObject.SetActive(false);
    }

    public void SetSoundVolume(float f) {
        soundVolumeLabel.text = "Sound Volume: " + Mathf.Round(f * 100f) + "%";
        PlayerPrefs.SetFloat("SoundVolume", f);
        soundAudioMixer.SetFloat("Volume", Mathf.Log10(f) * 20f);
    }

    public void SetMusicVolume(float f) {
        musicVolumeLabel.text = "Music Volume: " + Mathf.Round(f * 100f) + "%";
        PlayerPrefs.SetFloat("MusicVolume", f);
        musicAudioMixer.SetFloat("Volume", Mathf.Log10(f) * 20f);
    }

    public void SetResolution(int i) {
        Resolution resolution = resolutions[i];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool b) {
        PlayerPrefs.SetInt("UseFullScreen", (b ? 1 : 0));
        Screen.fullScreen = b;
    }

    public static void UpdateAudioPreference() {
        if (Instance != null) {
            Instance.soundAudioMixer.SetFloat("Volume", Mathf.Log10(PlayerPrefs.GetFloat("SoundVolume", 1f)) * 20f);
            Instance.musicAudioMixer.SetFloat("Volume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 1f)) * 20f);
        }
    }

    public static void SetMusic(AudioClip ac) {
        if (Instance != null) {
            Instance.music.clip = ac;
            Instance.lowPassFilter.cutoffFrequency = 22000f;
            Instance.music.volume = 1f;
            Instance.music.Play();
        }
    }

    public static void SetMusicInGameVolume(float f) {
        if (Instance != null) {
            Instance.music.volume = f;
        }
    }

    public static float GetMusicInGameVolume() {
        float output = 0f;

        if (Instance != null) {
            return Instance.music.volume;
        }

        return output;
    }

    public static bool MusicIsPlayingClip(AudioClip ac) {
        if (Instance != null) {
            return (Instance.music.clip == ac && Instance.music.isPlaying);
        }

        return false;
    }
}
