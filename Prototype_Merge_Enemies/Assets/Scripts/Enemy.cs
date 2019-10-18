using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, Interactable {

    public int tier = 1;

    private void Start() {
        Change(tier);
    }

    public void DropItem(Item i) {
        if(tier == i.tier) {
            Destroy(i.gameObject); //TODO: Object Pool
            Destroy(gameObject);
        }
    }

    public void PickItem(Item draggingObject) {}

    public void Change(int newTier) {
        tier = newTier;
        GetComponentInChildren<TMPro.TextMeshPro>().SetText("" + tier);
        //set image
    }
}
