using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, Enemy.IEnemyTargetable, IShopCustomer
{
	public static Player Instance { get; private set; }

	public event EventHandler OnGoldAmountChanged;

	private int goldAmount;

	public void BoughtItem(Item.ItemType itemType)
    {
		Debug.Log("Bought item: " + itemType);
		switch (itemType)
        {
			case itemType.ItemType.RedFlowerSeeds; break;
			case itemType.ItemType.BlueFlowerSeeds; break;
			case itemType.ItemType.GreenFlowerSeeds; break;

		}
    }

	public bool TrySpendGoldAmount(int spendGoldAmount) { 
		if (GetGoldAmount() >= spendGoldAmount)
        {
			goldAmount -= spendGoldAmount;
			OnGoldAmountChanged?.Invoke(this, EventArgs.Empty);
			return true;
        } else
        {
			return false;
        }
	}

 }