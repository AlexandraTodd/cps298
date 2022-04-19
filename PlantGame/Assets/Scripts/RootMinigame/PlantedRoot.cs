using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantedRoot : MonoBehaviour {
    public LineRenderer stem;
    [HideInInspector] public bool hasFlower = false;

    private void Start() {
        RootMinigameManager.Instance.plantedRoots.Add(this);
    }
}

