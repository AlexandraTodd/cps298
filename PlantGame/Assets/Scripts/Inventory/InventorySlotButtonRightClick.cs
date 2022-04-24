using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotButtonRightClick : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            //Debug.Log("Right clicked" + this);
            InventorySlot currentSlot = (transform.parent.gameObject).GetComponent<InventorySlot>();
            try { currentSlot.SellButton(); }
            catch { Debug.Log("Nothing to Sell"); }
        }
    }
}
