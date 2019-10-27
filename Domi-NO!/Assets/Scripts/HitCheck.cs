using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCheck : MonoBehaviour
{
    private bool isHit = false;
    public string requiredTag;
    public bool IsHit() { return isHit; }

    private void OnCollisionEnter(Collision collision) {
        if(requiredTag==null || requiredTag=="" || collision.gameObject.tag == requiredTag) { 
            isHit = true;
        }
        
    }
}
