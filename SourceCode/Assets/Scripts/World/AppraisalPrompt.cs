using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AppraisalPrompt : MonoBehaviour {
    public Image[] starBackgrounds;
    public Image[] starFills;
    public TMP_Text[] scoreDescriptions;
    public TMP_Text personalBestDescription;
    public TMP_Text moneyValue;
    public GameObject newPersonalBestNotification;
    public Button harvestSellButton;

    public void UpdateValues(float[] incomingValues, bool pbExists, bool isPb, float previousBest) {
        for (int i = 0; i < incomingValues.Length; i++) {
            switch (i) {
                // Variety
                case 0:
                    scoreDescriptions[i].text = Mathf.Round(incomingValues[0] * 36f).ToString("F0") + " / 36 unique colors planted";
                    break;

                // Volume
                case 1:
                    scoreDescriptions[i].text = Mathf.Round(incomingValues[1] * 50f).ToString("F0") + " / 50 total flowers planted";
                    break;

                // Vibrance
                case 2:
                    scoreDescriptions[i].text = Mathf.Round(incomingValues[2] * 3f).ToString("F2") + " / 3 average nutrients per flower";
                    break;

                // View
                case 3:
                    scoreDescriptions[i].text = Mathf.Round(incomingValues[3] * 20f).ToString("F2") + " / 20 cm kept between flowers";
                    break;

                // Final Score (Value)
                case 4:
                    scoreDescriptions[i].text = incomingValues[4] + " out of 5 stars";
                    break;
            }

            // Money is simply stars times 20, rounded to nearest dollar
            moneyValue.text = "$"+Mathf.Round(incomingValues[4] * 20f);

            // Disables harvest/sell button if it wouldn't do anything
            harvestSellButton.interactable = incomingValues[4] > 0f;

            // Stars have some quirks with their visuals that would be better to handle with a method
            // The divider is divide by 5 if its the total value, since thats the only one that comes already converted to a 5 star format
            SetStarFill(i, incomingValues[i] / (i == incomingValues.Length-1 ? 5f : 1f));
        }

        // If it's a PB...
        if (pbExists) {
            if (isPb) {
                newPersonalBestNotification.SetActive(true);
                personalBestDescription.text = "Previous Personal Best: ";
            } else {
                newPersonalBestNotification.SetActive(false);
                personalBestDescription.text = "Personal Best: ";
            }

            personalBestDescription.text += previousBest.ToString("F2");
        } else {
            newPersonalBestNotification.SetActive(false);
            personalBestDescription.text = "";
        }
    }

    public void SetStarFill(int index, float value) {
        // An extra 12% is lost between gaps. Gaps will be filled procedurally, but the potential must be offset by removing the 12%
        float starFillValue = value * 0.88f;
        if (starFillValue >= 0.17f) starFillValue += 0.03f;
        if (starFillValue >= 0.37f) starFillValue += 0.03f;
        if (starFillValue >= 0.57f) starFillValue += 0.03f;
        if (starFillValue >= 0.77f) starFillValue += 0.03f;

        starFills[index].fillAmount = Mathf.Min(1f, starFillValue);
    }
}
