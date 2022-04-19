using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    public static PauseMenu Instance;
    public Canvas menuCanvas;

    public Image blackTintBackground;
    public Button unpauseButton;
    public Button continueGameButton;
    public Button newGameButton;
    public Button exitToMenuButton;

    private void Awake() {
        // Singleton, important for returning to main menu from mid-game
        if (Instance != null) {
            Destroy(this.gameObject);
        } else Instance = this;

        DontDestroyOnLoad(Instance);
        ConfigureButtons(false);
    }

    public void Update() {
        if (SceneManager.GetActiveScene().name == "TownMap") {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                TogglePauseMenu(!menuCanvas.enabled);
            }
        }
    }

    public void TogglePauseMenu(bool enabled) {
        menuCanvas.enabled = enabled;
    }

    public void NewGame() {
        // Check for save files and delete
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
    }
}
