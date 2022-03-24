using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeMonkey.Utils;

public class UI_Shop : MonoBehaviour {

    private Transform container;
    private Transform shopItemTemplate;
    private IShopCustomer shopCustomer;

    private void Awake() {
        container = transform.Find("container");
        shopItemTemplate = container.Find("shopItemTemplate");
        shopItemTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        CreateItemButton(Item.GetSprite(Item.ItemType.RedFlowerSeeds), "Red Flower Seeds", itemSprite.GetCost(Item.ItemType.RedFlowerSeeds), 0);
        CreateItemButton(Item.GetSprite(Item.ItemType.BlueFlowerSeeds), "Blue Flower Seeds", itemSprite.GetCost(Item.ItemType.BlueFlowerSeeds), 1);
        CreateItemButton(Item.GetSprite(Item.ItemType.GreenFlowerSeeds), "Green Flower Seeds", itemSprite.GetCost(Item.ItemType.GreenlowerSeeds), 2);

        Hide();
    }

    private void CreateItemButton(Sprite itemSprite, string itemName, int itemCost) {
        Transform shopItemTransform = Instantiate(shopItemTemplate, container);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemHeight = 30f;
        shopItemRectTransform.anchoredPostion = new Vector2(0, -shopItemHeight * positionIndex);

        shopItemTransform.Find("itemName").GetComponet<TextMeshProUGUI>().SetText(itemName);
        shopItemTransform.Find("costText").GetComponet<TextMeshProUGUI>().SetText(itemCost.ToString());

        shopItemTransform,Find("itemImage").GetComponent<Image>().sprite = itemSprite;

        shopItemTransform.GetComponent<Button_UI>().ClickFunc = () =>
        {
            //CLicked on shop item button
            TryBuyItem(itemType);
        };
    }

    private void TryBuyItem(Item.ItemType itemType) {
        shopCustomer.TrySpendGoldAmount(itemType.GetCost(itemType))
        // Can afford cost
        shopCustomer.BoughtItem(itemType);
    }

    public void Show(IShopCustomer shopCustomer)
    {
        this.shopCustomer = shopCustomer;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
