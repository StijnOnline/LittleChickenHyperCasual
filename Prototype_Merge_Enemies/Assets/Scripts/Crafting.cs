using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour, Interactable {
    Item item1;
    Item item2;

    public Vector3 item1Pos;
    public Vector3 item2Pos;
    public Vector3 craftedPos;

    public GameObject poof;

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

        
        item1.GetComponent<Collider2D>().enabled = false;
        item2.GetComponent<Collider2D>().enabled = false;


        //animate
        Destroy(GameObject.Instantiate(poof, transform.position, transform.rotation), 2f);

        yield return new WaitForSeconds(1);

        //spawn new item
        GameManager.itemPool.Return(item1.gameObject);
        item2.Change(item1.tier + 1);
        item2.gameObject.layer = LayerMask.NameToLayer("Item");
        item2.GetComponent<Collider2D>().enabled = true;
        item2.transform.position = transform.position + craftedPos + new Vector3(Random.Range(-0.2f,0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));

        item1 = null;
        item2 = null;
    }

    public void PickItem(Item i)
    {
        if (i == item1)
        {
            item1 = null;
        }
    }
}