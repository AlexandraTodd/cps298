using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControlScript : MonoBehaviour {

	public Text moneyText;
	public Text seedsText;
	public static int moneyAmount;
	public static int seedsAmount;
	//int isRifleSold;
	//int isSeedsSold;
	//public GameObject rifle;
	//public GameObject seeds;

	// Use this for initialization
	void Start () {
		moneyAmount = PlayerPrefs.GetInt ("MoneyAmount");
		seedsAmount = PlayerPrefs.GetInt("SeedsAmount");
		/*isRifleSold = PlayerPrefs.GetInt ("IsRifleSold");
		
		if (isRifleSold == 1)
			rifle.SetActive (true);
		else
			rifle.SetActive (false);
		*/

		/*
		isSeedsSold = PlayerPrefs.GetInt("IsSeedsSold");

		if (isSeedsSold == 1)
			seeds.SetActive(true);
		else
			seeds.SetActive(false);
		*/
	}
	
	// Update is called once per frame
	void Update () {
		moneyText.text = "Money: " + moneyAmount.ToString() + "$";
		seedsText.text = "Seeds: " + seedsAmount.ToString();
	}

	public void gotoShop()
	{
		PlayerPrefs.SetInt ("MoneyAmount", moneyAmount);
		PlayerPrefs.SetInt("SeedsAmount", seedsAmount);
		SceneManager.LoadScene ("ShopScene");
	}
}
