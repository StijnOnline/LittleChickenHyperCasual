using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Game Settings")]
    [SerializeField] private float dist = 0f;
    [SerializeField] private float targetDist = 0.2f;
    [SerializeField] private float targetWidth = 0.18f;
    [SerializeField] private float startSpeed = 0.35f;
    private float speed = 0;
    private int score = 0;
    private int highScore = 0;
    private float lastPos = -0.1f;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private Vector3 camOffset;


    [Header("Important References")]
    [SerializeField] private Path path;
    [SerializeField] private TMPro.TextMeshProUGUI debugText;

    [SerializeField] private GameObject dominoPrefab;
    private Transform currentDomino;
    [SerializeField] private Transform target;
    [SerializeField] private Transform cam;

    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI highScoreText;

    public static GameObjectPool dominoPool;
    List<GameObject> dominoes = new List<GameObject>();



    private enum GameState {Begin, Playing, Ended };
    private GameState gameState = GameState.Begin;

    void Start()
    {
        dominoPool = new GameObjectPool(dominoPrefab, "dominoPool");

        //target = GameObject.Instantiate(target);
        currentDomino =  dominoPool.GetNext().transform;
        dominoes.Add(currentDomino.gameObject);

        //highScore = PlayerPrefs.GetInt("HighScore", 0);
        //highScoreText.SetText(""+highScore);

        

        speed = startSpeed;
    }

    public void Update() {
        

        //TODO: Add easy double tap by waiting
        bool inputL = false;
        bool inputR = false;
        if(Input.touchCount > 0) {
            for(int i = 0; i < Input.touchCount; i++) {
                if(Input.GetTouch(i).position.x < Screen.width / 2f) {
                    inputL = true;
                    debugText.SetText("Left");
                } else {
                    inputR = true;
                    debugText.SetText("Right");
                }
            }
        }
        if(inputL && inputR) {
            debugText.SetText("Both");
        }

        if(gameState == GameState.Begin && Input.touchCount > 0) { gameState = GameState.Playing; }

        if(gameState == GameState.Playing) {
            dist += Time.deltaTime * speed;
            if(path.getLength() > dist) { cam.position = path.Evaluate(dist) + camOffset; } else {
                //end of path
            }

            //    speed = startSpeed + speedCurve.Evaluate(score);

            //    dist += Time.deltaTime * speed;
            //    cam.position = new Vector3(dist - 1, 0.6f, -1f);
            //    currentDomino.position = new Vector3(dist, 0.1f, 0);

            //    bool touched = Input.touchCount > 0;
            //    if(touched) touched = Input.GetTouch(0).phase == TouchPhase.Began;

            //    if((touched || Input.GetMouseButtonDown(0)) && (dist > lastPos + 0.1f)) {
            //        PlaceDomino();
            //    } else if(dist > target.position.x + (targetWidth / 2f)) {
            //        StartCoroutine(ShowTextDuration("Too late!", 0.5f));
            //        StartCoroutine(EndGame());
            //    }
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
        if(score > highScore) {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            highScoreText.SetText("" + highScore);
        }

        foreach(GameObject obj in dominoes) {
            obj.GetComponent<Rigidbody>().isKinematic = false;
        }

        Vector3 targetPos = new Vector3(-1, Mathf.Max(dist / 2f, 0.6f), Mathf.Min( -dist/2f,-1f));

        while((cam.position - targetPos).magnitude > 0.05f) {
            cam.position = Vector3.Lerp(cam.position, targetPos, 0.05f);
            yield return 0;
        }

        yield return new WaitForSeconds(1);

        Debug.Log("first",dominoes[0]);
        dominoes[0].GetComponent<Rigidbody>().AddForce(Vector3.right * 100f);

        yield return new WaitForSeconds(0.3f * dist / targetDist + 1f);
                
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
        speed = startSpeed;
        scoreText.SetText("" + score);

        cam.position = new Vector3(dist - 1, 0.6f, -1f);

        currentDomino = dominoPool.GetNext().transform;
        dominoes.Add(currentDomino.gameObject);
        currentDomino.position = new Vector3(dist, 0.1f, 0);

        target.position = new Vector3(0, -0.28f, 0);

        gameState = GameState.Begin;
    }
}