using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopControlScript : MonoBehaviour {

	int moneyAmount;
	int seedsAmount;
	//int isRifleSold;

	public Text moneyAmountText;
	public Text seedsAmountText;
	//public Text riflePrice;
	public Text seedPrice;

	public Button buyButton;
	public Button sellButton;

	// Use this for initialization
	void Start () {
		moneyAmount = PlayerPrefs.GetInt ("MoneyAmount");
		seedsAmount = PlayerPrefs.GetInt("SeedsAmount");
	}
	
	// Update is called once per frame
	void Update () {
		
		moneyAmountText.text = "Money: " + moneyAmount.ToString() + "$";
		seedsAmountText.text = "Seeds: " + seedsAmount.ToString();

		//isRifleSold = PlayerPrefs.GetInt ("IsRifleSold");

		//if (moneyAmount >= 5 && isRifleSold == 0)

		buyButton.interactable = moneyAmount >= 10;

		sellButton.interactable = seedsAmount >= 1;

	}

	//public void buyRifle()
	public void buySeeds()
	{
		moneyAmount -= 10;
		seedsAmount += 1;
		//PlayerPrefs.SetInt ("IsRifleSold", 1);
		//riflePrice.text = "Sold!";
		//seedPrice.text = "Sold!";
		//buyButton.gameObject.SetActive (false);
	}

	public void sellSeeds()
	{
		moneyAmount += 5;
		seedsAmount -= 1;
	}

	public void exitShop()
	{
		PlayerPrefs.SetInt ("MoneyAmount", moneyAmount);
		PlayerPrefs.SetInt("SeedsAmount", seedsAmount);
		SceneManager.LoadScene ("GameScene");
	}

	public void resetPlayerPrefs()
	{
		moneyAmount = 0;
		buyButton.gameObject.SetActive (true);
		//riflePrice.text = "Price: 5$";
		seedPrice.text = "Price: 10$";
		PlayerPrefs.DeleteAll ();
	}

}
