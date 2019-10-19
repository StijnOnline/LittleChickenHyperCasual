using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, Interactable {

    public int tier = 1;
    public float attackTimer = 10f;
    [SerializeField] private Transform attackbar;


    public delegate void EnemyEvent(int enemyTier);
    public EnemyEvent Attacked;
    public EnemyEvent Killed;

    private void Start() {
        Change(tier);
        StartCoroutine(Attack());
    }

    public void DropItem(Item i) {
        if(tier == i.tier) {
            Killed(tier);
            GameManager.itemPool.Return(i.gameObject);
            GameManager.enemyPool.Return(gameObject);
        }
    }

    public void PickItem(Item draggingObject) {}

    public void Change(int newTier) {
        tier = newTier;
        GetComponentInChildren<SpriteRenderer>().sprite = GameManager.enemyData[newTier-1].enemySprite;
        attackTimer = GameManager.enemyData[newTier-1].attackTimer;
    }

    public IEnumerator Attack() {
        float t = 0;
        while(t < attackTimer) {
            attackbar.localScale = new Vector3( (attackTimer-t) /attackTimer ,0.1f,1);

            t += Time.deltaTime;
            yield return 0;
        }

        Attacked(tier);

        StartCoroutine(Attack());
    }
}
