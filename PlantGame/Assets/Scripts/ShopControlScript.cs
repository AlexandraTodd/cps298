using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopControlScript : MonoBehaviour {

	int moneyAmount;
	//int isRifleSold;

	public Text moneyAmountText;
	//public Text riflePrice;
	public Text seedPrice;

	public Button buyButton;

	// Use this for initialization
	void Start () {
		moneyAmount = PlayerPrefs.GetInt ("MoneyAmount");
	}
	
	// Update is called once per frame
	void Update () {
		
		moneyAmountText.text = "Money: " + moneyAmount.ToString() + "$";

		//isRifleSold = PlayerPrefs.GetInt ("IsRifleSold");

		//if (moneyAmount >= 5 && isRifleSold == 0)
		if (moneyAmount >= 10)
			buyButton.interactable = true;
		else
			buyButton.interactable = false;	
	}

	//public void buyRifle()
	public void buySeeds()
	{
		moneyAmount -= 10;
		//PlayerPrefs.SetInt ("IsRifleSold", 1);
		//riflePrice.text = "Sold!";
		seedPrice.text = "Sold!";
		buyButton.gameObject.SetActive (false);
	}

	public void exitShop()
	{
		PlayerPrefs.SetInt ("MoneyAmount", moneyAmount);
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
