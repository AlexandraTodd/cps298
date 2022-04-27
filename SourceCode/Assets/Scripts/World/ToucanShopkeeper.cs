using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ToucanShopkeeper : MonoBehaviour {
    public SpriteRenderer highlightSprite;
    public TMP_Text moneyDisplayText;
    [HideInInspector] public float flashingAnimation = 0f;

    void Start() {
        moneyDisplayText.text = "You have:\n$";

        int currencyValue = 25;

        // Attempt to load money
        string currencyPath = Application.persistentDataPath + "/currency.dat";
        if (File.Exists(currencyPath)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(currencyPath, FileMode.Open);
            if (stream.Length != 0) {
                CurrencySave data = (CurrencySave)(formatter.Deserialize(stream));
                currencyValue = data.currency;
            }

            stream.Close();
        }

        moneyDisplayText.text += currencyValue;

        if (currencyValue == 0) OverworldManager.Instance.noMoney = true;
    }

    // Update is called once per frame
    void Update() {
        flashingAnimation += Time.deltaTime * 2f;

        // Plots of land with nutrients still available have a yellow glow that fades in and out
        Color rowAvailableColor = highlightSprite.color;
        rowAvailableColor.a = Mathf.Abs(Mathf.Sin(flashingAnimation)) * 0.25f;
        highlightSprite.color = rowAvailableColor;
    }

    void OnMouseDown() {
        if (OverworldManager.Instance.fadeOutAnimation == 0f) {
            if (PauseMenu.Instance) PauseMenu.Instance.playerPosition = new Vector3(transform.position.x, transform.position.y - 0.5f, 0f);
            OverworldManager.Instance.cinematicCameraTarget = transform;
            OverworldManager.Instance.TransitionToScene("InventoryFlower");
            OverworldManager.Instance.fadeOutAnimation = 0.5f;
        }
    }
}
