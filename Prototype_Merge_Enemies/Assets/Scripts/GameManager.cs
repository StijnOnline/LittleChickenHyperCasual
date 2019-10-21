using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    public static GameObjectPool itemPool;
    public static ItemData[] itemData;

    [SerializeField] private GameObject enemyPrefab;
    public static GameObjectPool enemyPool;
    public static EnemyData[] enemyData;

    private Player player;

    private System.Action EnemyAttackedAction;


    //TODO: make better spawn system
    private int[] spawnTiers = { 1,1,2,1,1,2,1,2,1,2,1,3};
    private int counter = 0;

    void Start()
    {
        itemPool = new GameObjectPool(itemPrefab,"itemPool");
        enemyPool = new GameObjectPool(enemyPrefab, "enemyPool");

        itemData = Resources.LoadAll<ItemData>("Weapons");
        enemyData = Resources.LoadAll<EnemyData>("Enemies");

        player = FindObjectOfType<Player>();
        EnemyAttackedAction += player.TakeDamage;

        //test
        SpawnItem(1, new Vector3(-1.5f,0,0));
        SpawnItem(1, new Vector3(-0.5f, 0, 0));
        SpawnItem(1, new Vector3(1.5f,0,0));
        SpawnItem(1, new Vector3(0.5f, 0, 0));

        StartCoroutine(EnemySpawnLoop());
    }


    private IEnumerator EnemySpawnLoop() {
        SpawnEnemy(spawnTiers[counter], new Vector3((counter % 3) - 1, 3, 0));
        counter++;

        yield return new WaitForSeconds(8f);
        StartCoroutine(EnemySpawnLoop());
    }

    private void SpawnItem(int tier, Vector3 pos) {
        GameObject ob = itemPool.GetNext(); 
        ob.SetActive(true); 
        ob.transform.position = pos;
        ob.GetComponent<Item>().Change(tier);
    }

    private void SpawnEnemy(int tier, Vector3 pos) {
        Enemy ob = enemyPool.GetNext().GetComponent<Enemy>();
        ob.gameObject.SetActive(true);
        ob.transform.position = pos;
        ob.Change(tier);
        ob.Attacked += EnemyAttacked;
        ob.Killed += EnemyKilled;
    }

    private void EnemyAttacked(int tier) {
        EnemyAttackedAction.Invoke();
    }

    private void EnemyKilled(int tier) {
        for(int i = 0; i < tier; i++) {
            SpawnItem(1, new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 0));
        }
        //float r = Random.Range(0f, 1f);
        //if(r > 0.6f) { SpawnItem(2, new Vector3(0, 0, 0)); }
        //else{ SpawnItem(1, new Vector3(0, 0, 0)); }
    }
}