using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantedRoot : MonoBehaviour {
    public LineRenderer stem;

    private void Start() {
        RootMinigameManager.Instance.plantedRoots.Add(this);
    }
}

