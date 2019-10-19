using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Data/EnemyData", order = 52)]
public class EnemyData : ScriptableObject {
    public Sprite enemySprite;
    public float attackTimer;
}