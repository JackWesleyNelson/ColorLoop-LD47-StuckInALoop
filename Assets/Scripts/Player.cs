using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//State corresponding to different keyboard, ps controller and xbox controller inputs.
public enum PlayerState
{
    Q, W, E, R,
    None
}

public class Player : MonoBehaviour
{
    [SerializeField]
    private Game game = null;

    #region Player
    [SerializeField]
    private GameObject player = null;

    [SerializeField]
    private AudioSource bounceAuidoSource = null;
    [SerializeField]
    private AudioClip death = null;


    //Number of degrees per second to travel around a circle.
    [SerializeField]
    private float radialSpeed = 20f;
    [SerializeField]
    private float jumpSpeed = .5f;
    [SerializeField]
    private float jumpSpeedIncrease = .01f;
    [SerializeField]
    private float maxJumpSpeed = 4f;
    private bool isJumping = false;

    private float degrees = -90;

    //Distance from center while on inner and outer edges.
    private float radiusInner = 4.05f;
    private float radiusOuter = 4.75f;
    private float radiusCurrent;

    private bool isOuter = false;
    private bool destinationReached = false;

    [SerializeField]
    private List<Material> playerMaterials = null;
    private PlayerState playerState = PlayerState.None;

    private float score = 0;
    private float highScoreVeryEasy, highScoreEasy, highScoreMedium, highScoreHard, highScoreVeryHard;
    [SerializeField]
    TMPro.TextMeshProUGUI diffAndScoreDisplay= null;
    #endregion

    private void Start()
    {
        radiusCurrent = radiusOuter;
        highScoreVeryEasy = PlayerPrefs.GetFloat("highScoreVeryEasy");
        highScoreEasy = PlayerPrefs.GetFloat("highScoreEasy");
        highScoreMedium = PlayerPrefs.GetFloat("highScoreMedium");
        highScoreHard = PlayerPrefs.GetFloat("highScoreHard");
        highScoreVeryHard = PlayerPrefs.GetFloat("highScoreVeryHard");
    }

    void Update()
    {
        HandlePlayerInput();
        HandlePlayerColorState();
        HandlePlayerPosition();
        UpdateScoreBoard();
    }

    private void UpdateScoreBoard()
    {
        diffAndScoreDisplay.text = GetScoreBoardDisplayText();
    }

    public string GetScoreBoardDisplayText()
    {
        return $"\n\n{GetDifficultyString()}\n\nScore:\n{score}\n\nHigh Score:\n{GetHighScore()}";
    }

    private string GetDifficultyString()
    {
        switch (game.difficulty)
        {
            case Difficulty.VERY_EASY:
                return "Very Easy";
            case Difficulty.EASY:
                return "Easy";
            case Difficulty.MEDIUM:
                return "Medium";
            case Difficulty.HARD:
                return "Hard";
            case Difficulty.EXTREME:
                return "Very Hard";
            default:
                return "";
        }
    }

    private float GetHighScore()
    {
        switch (game.difficulty)
        {
            case Difficulty.VERY_EASY:
                return highScoreVeryEasy;
            case Difficulty.EASY:
                return highScoreEasy;
            case Difficulty.MEDIUM:
                return highScoreMedium;
            case Difficulty.HARD:
                return highScoreHard;
            case Difficulty.EXTREME:
                return highScoreVeryHard;
            default:
                return 0;
        }
    }

    private void UpdateHighScore()
    {
        switch (game.difficulty)
        {
            case Difficulty.VERY_EASY:
                highScoreVeryEasy = score;
                break;
            case Difficulty.EASY:
                highScoreEasy = score;
                break;
            case Difficulty.MEDIUM:
                highScoreMedium = score;
                break;
            case Difficulty.HARD:
                highScoreHard = score;
                break;
            case Difficulty.EXTREME:
                highScoreVeryHard = score;
                break;
            default:
                break;
        }
    }

    private void PlayerPrefsWriteHighScores()
    {
        PlayerPrefs.SetFloat("highScoreVeryEasy", highScoreVeryEasy);
        PlayerPrefs.SetFloat("highScoreEasy", highScoreEasy);
        PlayerPrefs.SetFloat("highScoreMedium", highScoreMedium);
        PlayerPrefs.SetFloat("highScoreHard", highScoreHard);
        PlayerPrefs.SetFloat("highScoreVeryHard", highScoreVeryHard);
    }

    private void HandlePlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerState = PlayerState.Q;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerState = PlayerState.W;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerState = PlayerState.E;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            playerState = PlayerState.R;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            //destinationReached = false;
            //isOuter = !isOuter;
        }
    }

    private void HandlePlayerColorState()
    {
        switch (playerState)
        {
            case PlayerState.Q:
                player.GetComponent<Renderer>().material = playerMaterials[0];
                break;
            case PlayerState.W:
                player.GetComponent<Renderer>().material = playerMaterials[1];
                break;
            case PlayerState.E:
                player.GetComponent<Renderer>().material = playerMaterials[2];
                break;
            case PlayerState.R:
                player.GetComponent<Renderer>().material = playerMaterials[3];
                break;
            default:
                player.GetComponent<Renderer>().material = playerMaterials[4];
                break;
        }
    }

    private void HandlePlayerPosition()
    {
        jumpSpeed = Mathf.Min(jumpSpeed + (Time.deltaTime*jumpSpeedIncrease), maxJumpSpeed);
        if (isJumping)
        {
            if (isOuter && !destinationReached && radiusCurrent < radiusOuter)
            {
                radiusCurrent += jumpSpeed * Time.deltaTime;
                if (radiusCurrent > radiusOuter)
                {
                    //destinationReached = true;
                    isOuter = !isOuter;
                    bounceAuidoSource.Play();
                }
            }
            else if (!destinationReached && radiusCurrent > radiusInner)
            {
                radiusCurrent -= jumpSpeed * Time.deltaTime;
                if (radiusCurrent < radiusInner)
                {
                    //destinationReached = true;
                    isOuter = !isOuter;
                    bounceAuidoSource.Play();
                }
            }
        }

        radiusCurrent = Mathf.Clamp(radiusCurrent, radiusInner, radiusOuter);

        degrees += radialSpeed * Time.deltaTime;
        Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, degrees) * Vector2.right);
        player.transform.position = dir * radiusCurrent;
    }

    public float GetDegrees()
    {
        return degrees;
    }

    public float GetRadialSpeed()
    {
        return radialSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(this.gameObject != null)
        {
            if (collision.gameObject.GetComponent<Note>().GetPlayerStateToEnter() != playerState)
            {
                //Store the current score, destroy the scene, and reload the scene
                PlayerPrefsWriteHighScores();
                StartCoroutine(Die());
                game.GameOver();
                //Destroy(collision.gameObject);
            }
            else
            {
                //Destroy the note
                Destroy(collision.gameObject);
                //Update Score
                score += 1f;
                if(score > GetHighScore())
                {
                    UpdateHighScore();
                }
            }
        }
    }

    private IEnumerator Die()
    {
        bounceAuidoSource.clip = death;
        bounceAuidoSource.Play();
        while (bounceAuidoSource.isPlaying)
        {
            yield return null;
        }
        Destroy(this.gameObject);
    }

}
