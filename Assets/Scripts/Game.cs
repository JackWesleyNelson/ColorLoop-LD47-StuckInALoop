using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Difficulty { VERY_EASY, EASY, MEDIUM, HARD, EXTREME }

//BUG: The Material color doesn't update until it's past a third of the ring for some reason.
public class Game : MonoBehaviour
{
    #region Routes
    //private float routeDifficulty = 0;
    //private Color routeColor = new Color(255, 0, 0);
    //[SerializeField]
    //private Material routeMaterialInner = null;
    //[SerializeField]
    //private Material routeMaterialOuter = null;

    [SerializeField]
    private GameObject routeInner = null;
    [SerializeField]
    private GameObject routeOuter = null;

    #endregion

    #region Notes
    [SerializeField]
    private GameObject noteContainer = null;

    [SerializeField]
    private GameObject noteDefault = null;

    private Queue<GameObject> notes = null;

    private float notesRadius = 4.4f;
    private float zRotationOffset = 90;
    private float timeBeforeFirstNote = 0.9f;
    private float difficultyTime = 0;
    private float distanceToSpawnAhead = 330f;
    private float lastSpawnedDegree = 0;

    #endregion


    private bool isGameStarted = true;

    [SerializeField]
    public Difficulty difficulty = Difficulty.MEDIUM;

    [SerializeField]
    Player player = null;
  
    [SerializeField]
    private GameObject uiTitle = null;
    [SerializeField]
    private GameObject uiMainMenu = null;
    [SerializeField]
    private Countdown countdown = null;

    private void Start()
    {
        #if UNITY_STANDALONE
        Application.quitting += () => PlayerPrefs.SetInt("SeenIntroThisSession", 0);
        #endif
        #if UNITY_EDITOR
        EditorApplication.quitting += () => PlayerPrefs.SetInt("SeenIntroThisSession", 0);
        //EditorApplication.playModeStateChanged += (PlayModeStateChange pmsc) => {if(pmsc == PlayModeStateChange.ExitingPlayMode || pmsc == PlayModeStateChange.EnteredPlayMode) PlayerPrefs.SetInt("SeenIntroThisSession", 0);};
        #endif

        if (PlayerPrefs.GetInt("SeenIntroThisSession") == 1)
        {
            uiMainMenu.SetActive(true);
        }
        else
        {
            uiTitle.SetActive(true);
            PlayerPrefs.SetInt("SeenIntroThisSession", 1);
        }

        difficulty = (Difficulty)PlayerPrefs.GetInt("Difficulty");
        
        notes = new Queue<GameObject>();
        StartCoroutine(StartMainMenu());
    }

    private IEnumerator StartMainMenu()
    {
        Animator animator = uiTitle.GetComponent<Animator>();
        //Wait for the animation to finish.
        while (AnimHelper.IsPlaying(animator, "TitleFadeIn"))
        {
            yield return null;
        }
        uiTitle.SetActive(false);
        uiMainMenu.SetActive(true);
    }

    public void BeginGame()
    {
        PlayerPrefs.SetInt("Difficulty", (int) difficulty);
        SetDifficultyTime();
        player.gameObject.SetActive(true);
        StartCoroutine(SpawnInitialNotes());
        isGameStarted = true;
    }

    private void Update()
    {
        if(notes.Count > 0 && notes.Peek() == null)
        {
            notes.Dequeue();
            GameObject note = SpawnNote(lastSpawnedDegree + difficultyTime * player.GetRadialSpeed());
            notes.Enqueue(note);
            //StartCoroutine(FadeInNote(note));
        }

    }

    public void GameOver()
    {
        foreach(GameObject note in notes)
        {
            if(note != null)
            {
                note.GetComponent<Rigidbody2D>().gravityScale = 1;
            }
        }
        countdown.GameOver();
        StartCoroutine(GameOverTasks());
    }

    public IEnumerator GameOverTasks()
    {
        Animator rinAnimator = routeInner.GetComponent<Animator>();
        Animator routAnimator = routeOuter.GetComponent<Animator>();

        rinAnimator.Play("InnerLoopFadeOut", 0, 0f);
        yield return null;
        routAnimator.Play("OuterLoopFadeOut", 0, 0f);
        yield return null;

        while (AnimHelper.IsPlaying(rinAnimator, "InnerLoopFadeOut") || AnimHelper.IsPlaying(routAnimator, "OuterLoopFadeOut"))
        {
            yield return null;
        }

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Game");

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    public void SetDifficultyTime() {
        switch (difficulty)
        {

            case Difficulty.VERY_EASY:
                difficultyTime = 1.75f;
                break;
            case Difficulty.EASY:
                difficultyTime = 1.25f;
                break;
            case Difficulty.MEDIUM:
                difficultyTime = .75f;
                break;
            case Difficulty.HARD:
                difficultyTime = .5f;
                break;
            case Difficulty.EXTREME:
                difficultyTime = .25f;
                break;
            default:
                Debug.LogError("Difficult not selected");
                break;
        }
    }

    private IEnumerator SpawnInitialNotes()
    {
        while (!isGameStarted)
        {
            yield return null;
        }

        float playerDegrees = player.GetDegrees();
        float playerSpeed = player.GetRadialSpeed();
        float distanceBetweenNotes = playerSpeed * difficultyTime;
        int notesSpawned = (int)(distanceToSpawnAhead / distanceBetweenNotes);
        float distanceBetweenFirstAndLastNote = 360 - (notesSpawned * distanceBetweenNotes);
        float distanceBeforeFirstNote = 3 * distanceBetweenFirstAndLastNote / 4;

        float startDistance = playerDegrees + distanceBeforeFirstNote;
        float endDistance = startDistance + distanceToSpawnAhead;

        float startTime = Time.time;

        for (float d = startDistance; d <= endDistance; d += distanceBetweenNotes)
        {
            notes.Enqueue(SpawnNote(d));
            yield return new WaitForSeconds(timeBeforeFirstNote / notesSpawned);
        }

        player.GetComponent<Rigidbody2D>().simulated = true;
        if(player.GetComponent<Animator>()?.enabled == true)
        {
            while (player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }
        }
        //If you don't destroy animator, it locks the material color, and position?
        Destroy(player.GetComponent<Animator>());
        player.GetComponent<Animator>().enabled = false;
        player.GetComponent<Player>().enabled = true;
        
    }

    private GameObject SpawnNote(float degreeToSpawn)
    {
        lastSpawnedDegree = degreeToSpawn;

        GameObject go = Instantiate(noteDefault, noteContainer.transform);
        PlayerState ps = (PlayerState)Random.Range(0, 4);
        Note note = go.GetComponent<Note>();
        note.SetPlayerStateRequiredToEnter(ps);
        Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, degreeToSpawn) * Vector2.right);
        go.transform.position = dir * notesRadius;
        go.transform.Rotate(new Vector3(0,0, degreeToSpawn + zRotationOffset));
        return go;
    }

    //private IEnumerator FadeInNote(GameObject note)
    //{
    //    MeshRenderer mr = note.GetComponent<MeshRenderer>();
    //    Material mat = mr.material;

    //    note.GetComponent<MeshRenderer>().material.EnableKeyword("_COLOR");
    //    note.GetComponent<MeshRenderer>().material.EnableKeyword("_ALPHATEST_ON");
    //    note.GetComponent<MeshRenderer>().material.EnableKeyword("_ALPHABLEND_ON");
    //    note.GetComponent<MeshRenderer>().material.EnableKeyword("_ALPHAPREMULTIPLY_ON");

    //    Color c = note.GetComponent<MeshRenderer>().material.color;

    //    c.a = 0;


    //    float fadeSpeed = .1f;
    //    while(c.a < 1)
    //    {
    //        c.a += fadeSpeed*Time.deltaTime;
    //        if(c.a > 1)
    //        {
    //            c.a = 1;
    //        }
    //        note.GetComponent<MeshRenderer>().material.color = c;
    //        yield return null;
    //    }
    //}

    ////Return a new color from the outside of the color wheel.
    //private Color NewColor()
    //{
    //    int r1 = Random.Range(0,6);
    //    float r2 = Random.Range(0f, 255f);
    //    switch (r1)
    //    {
    //        case 0:
    //            return new Color(255f, 0f, r2);

    //        case 1:
    //            return new Color(r2, 0f, 255f);

    //        case 2: 
    //            return new Color(0f, r2, 255f);

    //        case 3:
    //            return new Color(0f, 255f, r2);

    //        case 4:
    //            return new Color(r2, 255f, 0f);

    //        case 5:
    //            return new Color(255f, r2, 0f);

    //        default:
    //            Debug.LogError("New Color was not generated correctly.");
    //            return new Color(0, 0, 0);
    //    }
    //}

    ////Change the color of the circles over time.
    //private IEnumerator CycleRouteColorThroughColorWheel(float rateOfChange = 30f)
    //{
    //    routeColor = new Color(255, 0, 0);
    //    while (true)
    //    {
    //        //0
    //        while (routeColor.b < 255)
    //        {
    //            routeColor.b += GetColorShiftValue(rateOfChange);
    //            yield return null;
    //        }
    //        routeColor.b = 255;
    //        //1
    //        while (routeColor.r > 0)
    //        {
    //            routeColor.r -= GetColorShiftValue(rateOfChange);
    //            yield return null;
    //        }
    //        routeColor.r = 0;
    //        //2
    //        while (routeColor.g < 255)
    //        {
    //            routeColor.g += GetColorShiftValue(rateOfChange);
    //            yield return null;
    //        }
    //        routeColor.g = 255;
    //        //3
    //        while (routeColor.b > 0)
    //        {
    //            routeColor.b -= GetColorShiftValue(rateOfChange);
    //            yield return null;
    //        }
    //        routeColor.b = 0;
    //        //4
    //        while (routeColor.r < 255)
    //        {
    //            routeColor.r += GetColorShiftValue(rateOfChange);
    //            yield return null;
    //        }
    //        routeColor.r = 255;
    //        //5
    //        while (routeColor.g > 0)
    //        {
    //            routeColor.g -= GetColorShiftValue(rateOfChange);
    //            yield return null;
    //        }
    //        routeColor.g = 0;
    //    }
    //}

    //private float GetColorShiftValue(float rateOfChange)
    //{
    //    return (float)(rateOfChange * Time.deltaTime);
    //}

}
