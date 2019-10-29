using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCheck : MonoBehaviour
{
    public bool isHit = false;
    public string requiredTag;

    private void OnCollisionEnter(Collision collision) {
        if(requiredTag==null || requiredTag=="" || collision.gameObject.tag == requiredTag) { 
            isHit = true;
        }
        
    }
}
