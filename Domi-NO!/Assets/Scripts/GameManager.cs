using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private const float INPUT_LENGTH_VALUE = 0.1f;

    private const float TARGET_HEIGHT = 0.126f;
    private const float TARGET_SCALE_X = 0.5f;
    private const float TARGET_SCALE_Y = 0.5f;
    private const float TARGET_PERFECT_RANGE = 0.03f;
    private const float DOMINO_UNSET_HEIGTH = 0.7f;
    private const float DOMINO_SET_HEIGTH = 0.43f;
    private const float DOMINO_TRANSPARANCY = 0.3f;
    private const float MINIMUM_PLACE_DIST = 0.1f;
    private const float FAKE_SHADOW_HEIGTH = 0.127f;

    private float dist = 0f;
    private float speed = 0;
    private int levelLength = 0;
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
    [SerializeField] private float speedIncreasePerScore = 0.005f;
    [SerializeField] private float cornerTargetWidth = 0.2f;
    [SerializeField] private float cornerTargetDist = 0.2f;
    [SerializeField] private Vector3 camOffset;
    [SerializeField] private float camZoom;

    [Header("Important References")]
    [SerializeField] private Path path;
    [SerializeField] private ImageLoader image;
    [SerializeField] private GameObject dominoPrefab;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject fakeShadow;
    [SerializeField] private GameObject perfectParticlesPrefab;
    [SerializeField] private Transform cam;


    [Header("UI")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private Image leftTouch;
    [SerializeField] private Image RightTouch;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private TMPro.TextMeshProUGUI loseText;
    [SerializeField] private TMPro.TextMeshProUGUI percentage;
    [SerializeField] private GameObject playAgain;



    [Header("Audio")]
    [SerializeField] private AudioClip place;
    [SerializeField] private AudioClip perfect;
    [SerializeField] private AudioClip wrong;
    [SerializeField] public AudioClip fall1;
    [SerializeField] public AudioClip fall2;
    private AudioSource audioSource;



    private HitCheck hitCheck;
    //TODO: UI button
    //TODO: Audio
    //TODO: BUGS

    void Start() {
        audioSource = gameObject.AddComponent<AudioSource>();
        camScript = cam.GetComponent<Camera>();

        dominoPool = new GameObjectPool(dominoPrefab, "dominoPool");
        targetPool = new GameObjectPool(targetPrefab, "targetPool");
        fakeShadow = GameObject.Instantiate(fakeShadow);

        SetTargets();
        levelLength = targets.Count;

        dist = targets[0].dist;
        cam.position = path.Evaluate(dist) + camOffset;


        currentDomino = dominoPool.GetNext().transform;
        dominoes.Add(currentDomino.gameObject);
        currentDomino.position = path.Evaluate(dist) + new Vector3(0, DOMINO_UNSET_HEIGTH, 0);
        currentDomino.rotation = targets[0].transform.rotation;

        fakeShadow.transform.position = path.Evaluate(dist) + new Vector3(0, FAKE_SHADOW_HEIGTH, 0);
        fakeShadow.transform.rotation = targets[0].transform.rotation * Quaternion.Euler(90, 0, 0);

        Material mat = currentDomino.GetComponent<Renderer>().material;
        Color c = mat.GetColor("_BaseColor");
        c.a = DOMINO_TRANSPARANCY;
        mat.SetColor("_BaseColor", c);

        //highScore = PlayerPrefs.GetInt("HighScore", 0);
        //highScoreText.SetText(""+highScore);

        speed = startSpeed;
    }

    public void Update() {


        if(Input.touchCount > 0) {
            for(int i = 0; i < Input.touchCount; i++) {
                if(Input.GetTouch(i).phase == TouchPhase.Began) {
                    if(Input.GetTouch(i).position.x < Screen.width / 2f) {
                        lastLeftTouch = INPUT_LENGTH_VALUE;
                    } else {
                        lastRightTouch = INPUT_LENGTH_VALUE;
                    }
                }
            }
        }
        if(SystemInfo.deviceType == DeviceType.Desktop) {
            if(Input.GetMouseButtonDown(0)) { lastLeftTouch = INPUT_LENGTH_VALUE; }
            if(Input.GetMouseButtonDown(1)) { lastRightTouch = INPUT_LENGTH_VALUE; }
        }

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

        if(gameState == GameState.Begin && input != TouchInput.None) { gameState = GameState.Playing; }

        if(gameState == GameState.Playing) {
            //speed = startSpeed + score * speedIncreasePerScore;
            dist += Time.deltaTime * speed;
            if(dist < path.getLength()) {
                cam.position = path.Evaluate(dist) + camOffset;
            } else {
                gameState = GameState.Ended;
                StartCoroutine(EndGame(true));
            }
        }
        if(gameState == GameState.Playing) {
            progressBar.value = (levelLength - targets.Count) / (float)levelLength;


            currentDomino.rotation = targets[0].transform.rotation;
            currentDomino.position = path.Evaluate(dist) + new Vector3(0, DOMINO_UNSET_HEIGTH, 0);
            if(targets[0].direction == TouchInput.Both) { currentDomino.position += currentDomino.right * 0.2f; }
            fakeShadow.transform.rotation = targets[0].transform.rotation * Quaternion.Euler(90, 0, 0);
            fakeShadow.transform.position = path.Evaluate(dist) + new Vector3(0, FAKE_SHADOW_HEIGTH, 0);
            if(targets[0].direction == TouchInput.Both) { fakeShadow.transform.position += fakeShadow.transform.right * 0.2f; }

            Color target = new Color(1, 1, 1, 0.5f);
            if(input == TouchInput.Left || input == TouchInput.Both) { leftTouch.color = new Color(1, 1, 1, 0.8f); }
            if(input == TouchInput.Right || input == TouchInput.Both) { RightTouch.color = new Color(1, 1, 1, 0.8f); }
            leftTouch.color = Color.Lerp(leftTouch.color, target, 0.1f);
            RightTouch.color = Color.Lerp(RightTouch.color, target, 0.1f);

            if(input != TouchInput.None && (dist > lastPos + MINIMUM_PLACE_DIST)) {

                if(dist < targets[0].dist - targets[0].width / 2) {
                    Debug.Log("Too Early at " + dist + ". Target: " + targets[0].dist + ", Width " + targets[0].width, targets[0]);
                    loseText.SetText("Too Early!");
                    PlayAudio("wrong");
                    StartCoroutine(EndGame());
                } else {
                    if(targets[0].direction == input) {

                        if(Mathf.Abs(dist - targets[0].dist) < TARGET_PERFECT_RANGE) {
                            PlayAudio("perfect");
                            Destroy(GameObject.Instantiate(perfectParticlesPrefab, path.Evaluate(dist), targets[0].transform.rotation * Quaternion.Euler(-90, 0, 0)), 1f);
                        } else {
                            PlayAudio("place");
                        }
                    } else {
                        Debug.Log("Wrong at " + dist + " With " + input + ". Target: " + targets[0].dist + ", Width " + targets[0].width + ", Dir " + targets[0].direction, targets[0]);
                        loseText.SetText("Wrong Stone!");
                        PlayAudio("wrong");
                        StartCoroutine(EndGame());
                    }

                }
                PlaceDomino(input);

            } else if(dist > targets[0].dist + targets[0].width / 2) {
                Debug.Log("Too late at " + dist + ". Target: " + targets[0].dist + ", Width " + targets[0].width, targets[0]);
                loseText.SetText("Too Late!");
                PlayAudio("wrong");
                StartCoroutine(EndGame());
            }

            for(int i = Mathf.Max(0, dominoes.Count - 12); i < dominoes.Count; i++) {
                Material mat = dominoes[i].GetComponent<Renderer>().material;
                Color c = mat.GetColor("_BaseColor");
                c.a = Mathf.Min(1, c.a + 0.1f * Time.deltaTime);
                mat.SetColor("_BaseColor", c);
            }
        }
    }

    public void PlaceDomino(TouchInput input) {

        Vector3 rotation = Vector3.zero;

        if(input == TouchInput.Both) {
            if(targets.Count > 0) {
                rotation = targets[0].transform.rotation.eulerAngles;
            }
        } else if(input == TouchInput.Right) { rotation += new Vector3(0, 90, 0); }

        currentDomino.position = path.Evaluate(dist) + new Vector3(0, DOMINO_SET_HEIGTH, 0);
        currentDomino.rotation = Quaternion.Euler(rotation);
        if(input == TouchInput.Both) { currentDomino.position = currentDomino.position + currentDomino.right * 0.2f; }

        lastPos = dist;
        currentDomino = dominoPool.GetNext().transform;
        dominoes.Add(currentDomino.gameObject);

        Material mat = currentDomino.GetComponent<Renderer>().material;
        Color c = mat.GetColor("_BaseColor");
        c.a = DOMINO_TRANSPARANCY;
        mat.SetColor("_BaseColor", c);

        NextTarget();
        //    score += 1;
        //    scoreText.SetText("" + score);
    }

    public void SetTargets() {
        List<Transform> nodes = path.GetPath();

        float traveled = 0;
        for(int i = 1; i < nodes.Count; i++) {
            Vector3 ToNext = nodes[i - 1].position - nodes[i].position;


            float d = cornerTargetDist;
            while(d < ToNext.magnitude - minMaxDist.y - cornerTargetDist) {
                float gap = Random.Range(minMaxDist.x, minMaxDist.y);
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
                targets[targets.Count - 1].gameObject.SetActive(false);



                targets[targets.Count - 1].dist = traveled + d + gap;
                targets[targets.Count - 1].width = width;

                d += gap + width / 2;

            }
            traveled += ToNext.magnitude;

            targets.Add(targetPool.GetNext().GetComponent<Target>());

            Vector3 rotation = new Vector3(0, -45, 0);
            if(i < nodes.Count - 1) {
                Vector3 pos1 = nodes[i - 1].position;
                Vector3 pos2 = nodes[i].position;
                Vector3 dir = pos1 - nodes[i + 1].position;

                if((dir.x < 0 ^ dir.z > 0)) { rotation += new Vector3(0, 90, 0); }
                if((dir.x < 0 ^ (pos2 - pos1).x == 0)) { rotation += new Vector3(0, 180, 0); }
            }
            targets[targets.Count - 1].transform.rotation = Quaternion.Euler(rotation);
            targets[targets.Count - 1].transform.position = nodes[i].position + targets[targets.Count - 1].transform.right * 0.2f + new Vector3(0, TARGET_HEIGHT, 0);
            targets[targets.Count - 1].transform.localScale = new Vector3(TARGET_SCALE_X, TARGET_SCALE_Y, cornerTargetWidth);
            targets[targets.Count - 1].dist = traveled;
            targets[targets.Count - 1].width = cornerTargetWidth;
            targets[targets.Count - 1].direction = TouchInput.Both;


            targets[targets.Count - 1].gameObject.SetActive(false);
        }
        targets[0].gameObject.SetActive(true);
    }

    public void NextTarget() {
        if(targets.Count > 1) {
            targets[0].gameObject.SetActive(false);
            targetPool.Return(targets[0].gameObject);
            targets.RemoveAt(0);
            targets[0].gameObject.SetActive(true);
        }
    }

    public IEnumerator EndGame(bool win = false) {
        gameState = GameState.Ended;

        percentage.SetText(Mathf.RoundToInt((levelLength - targets.Count) / (float)levelLength * 100) + "%");
        loseScreen.SetActive(true);

        //if(score > highScore) {
        //    highScore = score;
        //    PlayerPrefs.SetInt("HighScore", highScore);
        //    highScoreText.SetText("" + highScore);
        //}


        foreach(Target t in targets) {
            targetPool.Return(t.gameObject);
        }
        targets.Clear();


        foreach(GameObject obj in dominoes) {
            obj.GetComponent<Rigidbody>().isKinematic = false;
            hitCheck = obj.AddComponent<HitCheck>();
            hitCheck.requiredTag = "Domino";

            if(obj.transform.localRotation == Quaternion.identity)
                hitCheck.clip = fall1;
            else
                hitCheck.clip = fall2;
        }




        Vector3 targetPos = path.transform.position + camOffset * 5f;
        Camera camScript = cam.GetComponent<Camera>();

        while((cam.position - targetPos).magnitude > 0.05f) {
            cam.position = Vector3.Lerp(cam.position, targetPos, 0.05f);
            camScript.orthographicSize = Mathf.Lerp(camScript.orthographicSize, camZoom, 0.05f);
            yield return 0;
        }
        dominoes[0].GetComponent<Rigidbody>().AddForce(dominoes[0].transform.forward * 100f);

        if(dominoes.Count > 2) {
            while(!hitCheck.isHit) {
                yield return 0;
            }
        }
        Destroy(hitCheck);
        yield return new WaitForSeconds(2f);

        if(win) {
            yield return new WaitForSeconds(image.Reveal() + 5f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        } else {
            playAgain.SetActive(true);
        }
    }

    public void ForceReset() {
        hitCheck.isHit = true;
        //Invoke("ResetGame", 1f);
    }

    public void ResetGame() {
        loseScreen.SetActive(false);
        playAgain.SetActive(false);

        foreach(GameObject obj in dominoes) {
            obj.GetComponent<Rigidbody>().isKinematic = true;
            obj.transform.rotation = Quaternion.identity;
        }
        dominoPool.Return(dominoes);
        dominoes.Clear();

        currentDomino = dominoPool.GetNext().transform;
        dominoes.Add(currentDomino.gameObject);

        Material mat = currentDomino.GetComponent<Renderer>().material;
        Color c = mat.GetColor("_BaseColor");
        c.a = DOMINO_TRANSPARANCY;
        mat.SetColor("_BaseColor", c);

        SetTargets();
        levelLength = targets.Count;


        dist = targets[0].dist;
        //score = 0;
        lastPos = -0.1f;
        speed = startSpeed;
        //scoreText.SetText("" + score);

        currentDomino.position = path.Evaluate(dist) + new Vector3(0, DOMINO_UNSET_HEIGTH, 0);
        currentDomino.rotation = targets[0].transform.rotation;

        fakeShadow.transform.position = path.Evaluate(dist) + new Vector3(0, FAKE_SHADOW_HEIGTH, 0);
        fakeShadow.transform.rotation = targets[0].transform.rotation * Quaternion.Euler(90, 0, 0);

        cam.position = path.Evaluate(dist) + camOffset;
        camScript.orthographicSize = 1;

        Invoke("BeginState", 0.1f);
    }

    public void BeginState() {
        gameState = GameState.Begin;
    }

    public void PlayAudio(string name) {
        switch(name) {
            case "place": audioSource.PlayOneShot(place); break;
            case "perfect": audioSource.PlayOneShot(perfect); break;
            case "wrong": audioSource.PlayOneShot(wrong); break;
        }
    }
}