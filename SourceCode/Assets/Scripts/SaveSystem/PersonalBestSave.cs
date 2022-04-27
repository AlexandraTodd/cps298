using UnityEngine;

[System.Serializable]

public class PersonalBestSave {
    public float previousBest;

    public PersonalBestSave(float f) {
        previousBest = f;
    }
}