using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool started = false;

    public float dist = 0f;
    public float targetDist = 0.2f;
    public float targetWidth = 0.18f;
    public float speed = 0.25f;

    public GameObject dominoPrefab;
    private Transform currentDomino;
    public Transform target;
    public Transform cam;
    public TMPro.TextMeshProUGUI text;


    void Start()
    {
        target = GameObject.Instantiate(target);
        currentDomino = GameObject.Instantiate(dominoPrefab).transform;
    }

    public void Update() {
        if(!started && (Input.touchCount > 0 || Input.GetMouseButtonDown(0))) { started = true; }

        if(started) {
            dist += Time.deltaTime * speed;
            cam.Translate(new Vector3(Time.deltaTime * speed, 0, 0), Space.World);
            currentDomino.position = new Vector3(dist, 0.1f, 0);
        }

        if(Input.touchCount > 0 || Input.GetMouseButtonDown(0)) { //TODO: on touch down
            PlaceDomino();
            MoveTarget();
        } else if(dist > target.position.x + (targetWidth / 2f)) {
            StartCoroutine(ShowTextDuration("Too late!", 0.5f));
            MoveTarget();
        }

        
    }

    public void PlaceDomino() {
        if(dist < target.position.x - (targetWidth / 2f)) {
            StartCoroutine(ShowTextDuration("Too Early!", 0.5f));
        } else if(dist - target.position.x < 0.05f) {
            StartCoroutine(ShowTextDuration("Super!", 0.5f));
        } else {
            StartCoroutine(ShowTextDuration("Nice!",0.5f));            
        }

        MoveTarget();
        currentDomino.position = new Vector3(dist, 0, 0);
        currentDomino = GameObject.Instantiate(dominoPrefab).transform;        
    }

    public void MoveTarget()
    {
        target.Translate(new Vector3(targetDist, 0,0),Space.World);
    }

    public IEnumerator ShowTextDuration(string _text,float _time) {
        text.SetText(_text);
        yield return new WaitForSeconds(_time);
        text.SetText("");
    }
}