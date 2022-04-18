using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RootMinigameBackButton : MonoBehaviour {
    private void OnMouseDown() {
        SceneManager.LoadScene("TownMap");
    }
}
