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

    void Start()
    {
        itemPool = new GameObjectPool(itemPrefab,"itemPool");
        enemyPool = new GameObjectPool(enemyPrefab, "enemyPool");

        itemData = Resources.LoadAll<ItemData>("Weapons");
        enemyData = Resources.LoadAll<EnemyData>("Enemies");

        Debug.Log(itemData.Length);
        Debug.Log(enemyData.Length);


        //test
        SpawnItem(1, new Vector3(0,0,0));
        SpawnItem(1, new Vector3(-1, 0, 0));
        SpawnItem(1, new Vector3(1, 0, 0));

        SpawnEnemy(1, new Vector3(-1, 3, 0));
        SpawnEnemy(2, new Vector3(1, 3, 0));

    }

    public void SpawnItem(int tier, Vector3 pos) {
        GameObject ob = itemPool.GetNext(); 
        ob.SetActive(true); 
        ob.transform.position = pos;
        ob.GetComponent<Item>().Change(tier);
    }

    public void SpawnEnemy(int tier, Vector3 pos) {
        GameObject ob = enemyPool.GetNext();
        ob.SetActive(true);
        ob.transform.position = pos;
        ob.GetComponent<Enemy>().Change(tier);
    }


}