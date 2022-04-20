using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class RootMinigameSave {
    // Binary objects do not support vectors, only floats, so they need to be separated and rebuilt
    public float[] xCoordinates;
    public float[] yCoordinates;
    public float[] segmentLengths;
    public int[] pointsPerLine;
    public int generationSeed;
    public int[] startingColor;
    public int[] endingColor;
    public float[] endWidth;
    public bool[] nutrientsEnabled;
    public int[] nutrientCount;

    public RootMinigameSave(Vector3[] pointsInput, int[] pointsPerLineInput, int generationSeedInput, int[] startingColorInput, int[] endingColorInput,
        float[] endWidthInput, bool[] nutrientsEnabledInput, int[] nutrientCountInput) {
        pointsPerLine = pointsPerLineInput;

        xCoordinates = new float[pointsInput.Length];
        yCoordinates = new float[pointsInput.Length];

        for (int i = 0; i < pointsInput.Length; i++) {
            xCoordinates[i] = pointsInput[i].x;
            yCoordinates[i] = pointsInput[i].y;
        }

        generationSeed = generationSeedInput;

        startingColor = startingColorInput;
        endingColor = endingColorInput;
        endWidth = endWidthInput;
        nutrientsEnabled = nutrientsEnabledInput;
        nutrientCount = nutrientCountInput;
    }
}
