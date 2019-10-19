using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public int tier = 1;

    private void Start() {
        Change(tier);
    }

    public void Change(int newTier) {
        tier = newTier;
        GetComponentInChildren<SpriteRenderer>().sprite = GameManager.itemData[newTier-1].itemSprite;
    }
}