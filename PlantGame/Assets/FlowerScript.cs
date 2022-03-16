using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerScript : MonoBehaviour
{
    [SerializeField] private Material myMaterial;
    private int flowerColor = 0;
    private int flowerIntensity = 1;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            flowerColor = (flowerColor + 1) % 6;
            Debug.Log(flowerColor);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            flowerIntensity = (flowerIntensity + 1) % 3;
            Debug.Log(flowerIntensity);
        }

        myMaterial.SetInt("inputColor", flowerColor);
        myMaterial.SetInt("inputIntensity", flowerIntensity);
    }
}
