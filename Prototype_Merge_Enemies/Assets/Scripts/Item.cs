using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public int tier = 1;

    public void Change(int newTier) {
        tier = newTier;
        GetComponentInChildren<TMPro.TextMeshPro>().SetText("" + tier);
        //set image
    }
}
