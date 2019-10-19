using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 3;

    public void TakeDamage() {
        health--;
        transform.GetChild(2).gameObject.SetActive(health == 3);
        transform.GetChild(1).gameObject.SetActive(health >= 2);
        transform.GetChild(0).gameObject.SetActive(health >= 1);
    }
}
