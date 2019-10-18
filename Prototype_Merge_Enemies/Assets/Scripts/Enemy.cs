using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, Interactable {

    public int tier = 1;
    public float attackTimer = 10f;


    private void Start() {
        Change(tier);
    }

    public void DropItem(Item i) {
        if(tier == i.tier) {
            GameManager.itemPool.Return(i.gameObject);
            GameManager.enemyPool.Return(gameObject);
        }
    }

    public void PickItem(Item draggingObject) {}

    public void Change(int newTier) {
        tier = newTier;
        GetComponentInChildren<TMPro.TextMeshPro>().SetText("" + tier);
        //set image
    }

    public IEnumerator startAttack() {
        yield return new WaitForSeconds(attackTimer); 

        //attack
    }
}
