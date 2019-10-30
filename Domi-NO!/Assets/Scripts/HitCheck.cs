using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCheck : MonoBehaviour
{
    public bool isHit = false;
    public string requiredTag;
    public AudioClip clip;

    private void OnCollisionEnter(Collision collision) {
        if(requiredTag==null || requiredTag=="" || collision.gameObject.tag == requiredTag) { 
            isHit = true;
            PlayAudio();            
        }        
    }

    public void PlayAudio() {
        gameObject.AddComponent<AudioSource>().PlayOneShot(clip);        
        Destroy(gameObject.GetComponent<AudioSource>(), 1f);
        Destroy(this, 1f);
    }

}
