using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class InventorySave {
    public int[] itemListType, itemListStack, itemListColor, itemListIntensity;

    public InventorySave(List<Item> items) {
        // Converts an incoming list of items into arrays we can save into a binary file and reconstruct into items later
        int numberOfItems = items.Count;
        itemListType = new int[numberOfItems];
        itemListStack = new int[numberOfItems];
        itemListColor = new int[numberOfItems];
        itemListIntensity = new int[numberOfItems];

        for (int i = 0; i < numberOfItems; i++) {
            itemListType[i] = items[i].itemType;
            itemListStack[i] = items[i].stackSize;
            itemListColor[i] = items[i].color;
            itemListIntensity[i] = items[i].intensity;
        }
    }

    public InventorySave(int[] i1, int[] i2, int[] i3, int[] i4) {
        itemListType = i1;
        itemListStack = i2;
        itemListColor = i3;
        itemListIntensity = i4;
    }
}