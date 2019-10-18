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
        //GetComponentInChildren<TMPro.TextMeshPro>().SetText("" + tier);
        GetComponentInChildren<SpriteRenderer>().sprite = GameManager.itemData[newTier].itemSprite;
    }
}
