using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameObjectPool itemPool;
    public static GameObjectPool enemyPool;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject enemyPrefab;

    void Start()
    {
        itemPool = new GameObjectPool(itemPrefab,"itemPool");
        enemyPool = new GameObjectPool(enemyPrefab, "enemyPool");

        
    }

    
}