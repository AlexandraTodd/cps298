using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{

	public enum SeedType
	{
		RedFlowerSeeds,
		BlueFlowerSeeds,
		GreenFlowerSeeds
	}

	public static int GetCost(ItemType itemType)
	{
		switch (itemType)
		{
			default:
			case ItemType.RedFlowerSeeds:	return 10;  //return = cost #
			case ItemType.BlueFlowerSeeds:	return 10;  //return = cost #
			case ItemType.GreenFlowerSeeds: return 10;	//return = cost #
		}
	}

	public static Sprite GetSprite(ItemType itemType)
	{

		switch (itemType)
		{
			default:
			case itemType.RedFlowerSeeds: return GameAsset.i.s_RedFlowerSeeds;
			case itemType.BlueFlowerSeeds: return GameAsset.i.s_BlueFlowerSeeds;
			case itemType.GreenFlowerSeeds: return GameAsset.i.s_GreenFlowerSeeds;
		}
	}

}