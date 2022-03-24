using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTriggerCollider : MonoBehaviour
{
	[SerializeField] private UI_Shop uiShop;

	private void OnTriggerEnter2D(Collider2D collider)
	{
		IShopCustomer shopCustomer = collider.GetComponent<IShopCustomer>();
		if (shopCustomer != null)
        {
			uiShop.Show(shopCustomer);
        }
	}

	private void OnTriggerExit2D(Collider2D collider)
    {
		IShopCustomer shopCustomer = collider.GetComponent<IShopCustomer>();
		if (shopCustomer != null)
        {
			uiShop.Hide();
        }
    }

}
