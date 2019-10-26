using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private const float TARGET_HEIGHT = 0.26f;
    private const float TARGET_SCALE_X = 0.5f;
    private const float TARGET_SCALE_Y = 0.5f;
    private const float TARGET_PERFECT_RANGE = 0.15f;
    private const float DOMINO_UNSET_HEIGTH = 0.7f;
    private const float DOMINO_SET_HEIGTH = 0.5f;
    private const float DOMINO_TRANSPARANCY = 0.5f;
    private const float MINIMUM_PLACE_DIST = 0.1f;

    private float dist = 0f;
    private float speed = 0;
    private int score = 0;
    private int highScore = 0;
    private float lastPos = -0.1f;
    private float lastLeftTouch;
    private float lastRightTouch;

    private Transform currentDomino;
    Camera camScript;

    private GameObjectPool dominoPool;
    private GameObjectPool targetPool;
    List<GameObject> dominoes = new List<GameObject>();
    List<Target> targets = new List<Target>();    

    public enum TouchInput { None, Left, Right, Both };
    private TouchInput input;
    private enum GameState { Begin, Playing, Ended };
    private GameState gameState = GameState.Begin;

    [Header("Game Settings")]
    
    [SerializeField] private Vector2 minMaxDist = new Vector2(0.1f, 0.5f);
    [SerializeField] private float minWidth = 0.1f;
    [SerializeField] private float startSpeed = 0.35f;
    [SerializeField] private float cornerTargetWidth = 0.2f;
    [SerializeField] private float cornerTargetDist = 0.2f;
    [SerializeField] private Vector3 camOffset;   

    [Header("Important References")]
    [SerializeField] private Path path;
    [SerializeField] private GameObject dominoPrefab;    
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform cam;   

    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI highScoreText;

    

    void Start() {
        camScript = cam.GetComponent<Camera>();

        dominoPool = new GameObjectPool(dominoPrefab, "dominoPool");
        targetPool = new GameObjectPool(targetPrefab, "targetPool");

        currentDomino = dominoPool.GetNext().transform;
        dominoes.Add(currentDomino.gameObject);
        Material mat = currentDomino.GetComponent<Renderer>().material;
        Color c = mat.color;
        c.a = DOMINO_TRANSPARANCY;
        mat.color = c;

        //highScore = PlayerPrefs.GetInt("HighScore", 0);
        //highScoreText.SetText(""+highScore);

        speed = startSpeed;

        SetTargets();

        dist = targets[0].dist;
    }

    public void Update() {


        if(Input.touchCount > 0) {
            for(int i = 0; i < Input.touchCount; i++) {
                if(Input.GetTouch(i).phase == TouchPhase.Began) {
                    if(Input.GetTouch(i).position.x < Screen.width / 2f) {
                        lastLeftTouch = 0.06f;
                    } else {
                        lastRightTouch = 0.06f;
                    }
                }
            }
        }

        if(Input.GetMouseButtonDown(0)) { lastLeftTouch = 0.06f; }
        if(Input.GetMouseButtonDown(1)) { lastRightTouch = 0.06f; }

        input = TouchInput.None;
        if(lastLeftTouch > 0 && lastLeftTouch < 0.04) {
            if(lastRightTouch > 0)
                input = TouchInput.Both;
            else
                input = TouchInput.Left;
        }
        if(lastRightTouch > 0 && lastRightTouch < 0.04) {
            if(lastLeftTouch > 0)
                input = TouchInput.Both;
            else
                input = TouchInput.Right;
        }
        lastLeftTouch = Mathf.Max(lastLeftTouch - Time.deltaTime, 0);
        lastRightTouch = Mathf.Max(lastRightTouch - Time.deltaTime, 0);



        if(gameState == GameState.Begin && (Input.touchCount > 0 || Input.GetMouseButtonDown(0))) { gameState = GameState.Playing; }

        if(gameState == GameState.Playing) {
            //    speed = startSpeed + speedCurve.Evaluate(score);

            currentDomino.position = path.Evaluate(dist) + new Vector3(0, DOMINO_UNSET_HEIGTH, 0); 

            if(input != TouchInput.None && (dist > lastPos + MINIMUM_PLACE_DIST)) {
                if(dist < targets[0].dist - targets[0].width / 2) {
                    Debug.Log("Too late at " + dist + ". Target: " + targets[0].dist + ", Width " + targets[0].width, targets[0]);
                    text.SetText("Too Early!");
                    StartCoroutine(EndGame());
                } else {
                    //TODO: Check piece
                    if(targets[0].direction == input) {

                        if(Mathf.Abs(dist - targets[0].dist) < TARGET_PERFECT_RANGE) {
                            text.SetText("Perfect!");
                        } else {
                            text.SetText("Nice!");
                        }
                    } else {
                        Debug.Log("Too late at " + dist + " With " + input + ". Target: " + targets[0].dist + ", Width " + targets[0].width + ", Dir " + targets[0].direction, targets[0]);
                        text.SetText("Wrong!");
                        StartCoroutine(EndGame());
                    }

                }

                
                Quaternion rotation = Quaternion.identity;
                if(input == TouchInput.Both) { rotation *= Quaternion.Euler(0, -45, 0); } //TODO: calculate rotation
                else if(input == TouchInput.Left) { rotation *= Quaternion.Euler(0, 90, 0); }
                PlaceDomino(rotation);

            } 
            else if(dist > targets[0].dist + targets[0].width / 2) {
                Debug.Log("Too late at "+ dist + ". Target: " + targets[0].dist + ", Width " + targets[0].width,targets[0]);
                text.SetText("Too late!");
                StartCoroutine(EndGame());
            }

            dist += Time.deltaTime * speed;
            if(path.getLength() > dist) { cam.position = path.Evaluate(dist) + camOffset; } else {
                //TODO: end of path
            }
        }
    }

    public void PlaceDomino(Quaternion rotation) {

        Material mat = currentDomino.GetComponent<Renderer>().material;
        Color c = mat.color;
        c.a = 1f;
        mat.color = c;
        currentDomino.position = path.Evaluate(dist) + new Vector3(0, DOMINO_SET_HEIGTH, 0);
        currentDomino.rotation = rotation;

        lastPos = dist;
        currentDomino = dominoPool.GetNext().transform;
        mat = currentDomino.GetComponent<Renderer>().material;
        c = mat.color;
        c.a = 0.5f;
        mat.color = c;
        dominoes.Add(currentDomino.gameObject);
        NextTarget();
        //    score += 1;
        //    scoreText.SetText("" + score);
    }

    public void SetTargets() {
        List<Transform> nodes = path.GetPath();

        float traveled = 0;
        for(int i = 1; i < nodes.Count; i++) {
            Vector3 ToNext = nodes[i - 1].position - nodes[i].position;
            

            float d = 0;
            while(d < ToNext.magnitude - minMaxDist.y - cornerTargetDist) {
                float gap = Random.Range(minMaxDist.x + minWidth / 2, minMaxDist.y - minWidth / 2);
                float width = Mathf.Max(gap - minMaxDist.x / 2, minMaxDist.y / 2 - gap);

                targets.Add(targetPool.GetNext().GetComponent<Target>());
                targets[targets.Count - 1].transform.position = nodes[i - 1].position - ToNext.normalized * (gap + d) + new Vector3(0, TARGET_HEIGHT, 0);
                targets[targets.Count - 1].transform.localScale = new Vector3(TARGET_SCALE_X, TARGET_SCALE_Y, width);
                if(ToNext.x != 0) {
                    targets[targets.Count - 1].transform.rotation = Quaternion.Euler(0, 90, 0);
                    targets[targets.Count - 1].direction = TouchInput.Right;
                } else {
                    targets[targets.Count - 1].direction = TouchInput.Left;
                }
                if(i > 3) { targets[targets.Count - 1].gameObject.SetActive(false); }

                

                targets[targets.Count - 1].dist = traveled + d + gap;
                targets[targets.Count - 1].width = width;
                
                d += gap + width / 2;

            }
            traveled += ToNext.magnitude;

            targets.Add(targetPool.GetNext().GetComponent<Target>());
            targets[targets.Count - 1].transform.position = nodes[i].position + new Vector3(0, TARGET_HEIGHT, 0);
            targets[targets.Count - 1].transform.rotation = Quaternion.Euler(0, 45, 0); //TODO calculate direction
            targets[targets.Count - 1].transform.localScale = new Vector3(TARGET_SCALE_X, TARGET_SCALE_Y, cornerTargetWidth);
            targets[targets.Count - 1].dist = traveled;
            targets[targets.Count - 1].width = cornerTargetWidth;
            targets[targets.Count - 1].direction = TouchInput.Both;


            if(i > 3) { targets[targets.Count - 1].gameObject.SetActive(false); }            
        }
        
    }

    public void NextTarget() {
        targets[0].gameObject.SetActive(false);
        targetPool.Return(targets[0].gameObject);
        targets.RemoveAt(0);

        targets[Mathf.Min(targets.Count - 1, 6)].gameObject.SetActive(true);
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

        Vector3 targetPos = path.transform.position + camOffset;
        float targetZoom = 5f;

        Camera camScript = cam.GetComponent<Camera>();

        while((cam.position - targetPos).magnitude > 0.05f) {
            cam.position = Vector3.Lerp(cam.position, targetPos, 0.05f);
            camScript.orthographicSize = Mathf.Lerp(camScript.orthographicSize, targetZoom,0.05f);
            yield return 0;
        }

        yield return new WaitForSeconds(1);

        dominoes[0].GetComponent<Rigidbody>().AddForce(Vector3.right * 100f);

        //TODO: onLastHit test
        yield return new WaitForSeconds(0.3f * dist / minMaxDist.y + 1f);

        ResetGame();
    }

    private void ResetGame() {
        foreach(GameObject obj in dominoes) {
            obj.GetComponent<Rigidbody>().isKinematic = true;
            obj.transform.rotation = Quaternion.identity;
        }
        dominoPool.Return(dominoes);
        dominoes.Clear();

        currentDomino = dominoPool.GetNext().transform;
        dominoes.Add(currentDomino.gameObject);
        currentDomino.position = path.Evaluate(dist) + new Vector3(0, DOMINO_UNSET_HEIGTH, 0);
        Material mat = currentDomino.GetComponent<Renderer>().material;
        Color c = mat.color;
        c.a = DOMINO_TRANSPARANCY;
        mat.color = c;

        foreach(Target t in targets) {
            dominoPool.Return(t.gameObject);
        }
        targets.Clear();
        SetTargets();


        dist = targets[0].dist;
        score = 0;
        lastPos = -0.1f;
        speed = startSpeed;
        scoreText.SetText("" + score);

        cam.position = path.Evaluate(dist) + camOffset;        
        camScript.orthographicSize = 1;

        gameState = GameState.Begin;
    }
}