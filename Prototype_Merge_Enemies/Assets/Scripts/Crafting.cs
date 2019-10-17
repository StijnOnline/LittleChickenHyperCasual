using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour, Interactable {
    Item item1;
    Item item2;

    public Vector3 item1Pos;
    public Vector3 item2Pos;

    public TMPro.TextMeshPro debugtext;

    public void DropItem(Item i) { // when item is dropped on this
        if(item1 == null) { 
            item1 = i; 
            item1.transform.position = transform.position + item1Pos;
            item1.gameObject.layer = LayerMask.NameToLayer("Item_NoCollision");
        }
        else if(item2 == null && item1.tier == i.tier) { 
            item2 = i; 
            item2.transform.position = transform.position + item2Pos;
            item2.gameObject.layer = LayerMask.NameToLayer("Item_NoCollision");
            StartCoroutine(CraftItem());
        } else { //either full or incompatible

        }

        //TODO: make sure items arent taken out
    }

    public IEnumerator CraftItem() {
        //animate
        
        debugtext.text += "\nCrafted 2 Tier " + item1.tier + " items";
        
        yield return new WaitForSeconds(1);

        //spawn new item
        Destroy(item1.gameObject); //TODO: Object Pooling
        item2.Change(item1.tier + 1);
        item2.gameObject.layer = LayerMask.NameToLayer("Item");
        item2.transform.position = transform.position + new Vector3(0, 1.5f, 0);

        item1 = null;
        item2 = null;
    }
}