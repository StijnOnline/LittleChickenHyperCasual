using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float dist = 0f;
    [SerializeField] private float targetDist = 0.2f;
    [SerializeField] private float targetWidth = 0.18f;
    [SerializeField] private float speed = 0.25f;
    [SerializeField] private float bonusSpeed = 0.01f;
    private int score = 0;
    private float lastPos = -0.1f;

    [SerializeField] private GameObject dominoPrefab;
    private Transform currentDomino;
    [SerializeField] private Transform target;
    [SerializeField] private Transform cam;

    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;

    public static GameObjectPool dominoPool;
    List<GameObject> dominoes = new List<GameObject>();

    private enum GameState {Begin, Playing, Ended };
    private GameState gameState = GameState.Begin;

    void Start()
    {
        dominoPool = new GameObjectPool(dominoPrefab, "dominoPool");

        target = GameObject.Instantiate(target);
        currentDomino =  dominoPool.GetNext().transform;
        dominoes.Add(currentDomino.gameObject);
    }

    public void Update() {
        if(gameState == GameState.Begin && (Input.touchCount > 0 || Input.GetMouseButtonDown(0))) { gameState = GameState.Playing; }

        if(gameState == GameState.Playing) {
            dist += Time.deltaTime * (speed + score * bonusSpeed);
            cam.position = new Vector3(dist - 1, cam.position.y, cam.position.z);
            currentDomino.position = new Vector3(dist, 0.1f, 0);

            bool touched = Input.touchCount > 0;
            if(touched) touched = Input.GetTouch(0).phase == TouchPhase.Began;

            if((touched || Input.GetMouseButtonDown(0)) && (dist > lastPos + 0.1f)) {
                PlaceDomino();
            } else if(dist > target.position.x + (targetWidth / 2f)) {
                StartCoroutine(ShowTextDuration("Too late!", 0.5f));
                StartCoroutine(EndGame());
            }
        }

    }

    public void PlaceDomino() {
        currentDomino.position = new Vector3(dist, 0, 0);        

        if(dist < target.position.x - (targetWidth / 2f)) {
            StartCoroutine(ShowTextDuration("Too Early!", 0.5f));
            StartCoroutine(EndGame());
        } else {
            StartCoroutine(ShowTextDuration("Nice!", 0.5f));
            lastPos = dist;
            score += 1;
            scoreText.SetText("" + score);
            currentDomino = dominoPool.GetNext().transform;
            dominoes.Add(currentDomino.gameObject);
            MoveTarget();
        }
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
    
    public IEnumerator EndGame() {
        gameState = GameState.Ended;

        foreach(GameObject obj in dominoes) {
            obj.GetComponent<Rigidbody>().isKinematic = false;
        }

        Vector3 targetPos = new Vector3(0, dist/2f , dist/2f);

        while(cam.position.x > 0) {
            cam.position = cam.position - new Vector3(0.04f, 0, 0);
            yield return 0;
        }

        yield return new WaitForSeconds(1);

        Debug.Log("first",dominoes[0]);
        dominoes[0].GetComponent<Rigidbody>().AddForce(Vector3.right * 100f);

        yield return new WaitForSeconds(0.6f);

        while(cam.position.x < dist - 1) {
            cam.position = cam.position + new Vector3(0.02f,0,0);
            yield return 0;
        }

        yield return new WaitForSeconds(2f);

        ResetGame();
    }

    private void ResetGame() {
        foreach(GameObject obj in dominoes) {
            obj.GetComponent<Rigidbody>().isKinematic = true;
            obj.transform.rotation = Quaternion.identity;
        }
        dominoPool.Return(dominoes);
        dominoes.Clear();

        dist = 0f;
        score = 0;
        lastPos = -0.1f;
        scoreText.SetText("" + score);

        cam.position = new Vector3(dist - 1, cam.position.y, cam.position.z);

        currentDomino = dominoPool.GetNext().transform;
        dominoes.Add(currentDomino.gameObject);
        currentDomino.position = new Vector3(dist, 0.1f, 0);

        target.position = new Vector3(0, -0.28f, 0);

        gameState = GameState.Begin;
    }
}