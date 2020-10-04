using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    private TMPro.TextMeshProUGUI textToUpdate = null;

    [SerializeField]
    private Game game = null;

    [SerializeField]
    private Player player = null;

    [SerializeField]
    private GameObject uiMainMenu = null;

    private void Start()
    {
        textToUpdate = GetComponent<TMPro.TextMeshProUGUI>();
        StartCoroutine(IEStartCountdown());
    }

    public void StartGame()
    {
        gameObject.SetActive(true);
        uiMainMenu.SetActive(false);
    }

    private IEnumerator IEStartCountdown()
    {
        Animator animator = GetComponent<Animator>();

        textToUpdate.text = "3";
        animator.Play("FadeIn", 0, 0f);
        yield return null;

        while (AnimHelper.IsPlaying(animator, "FadeIn"))
        {
            yield return null;
        }

        textToUpdate.text = "2";
        animator.Play("FadeIn", 0, 0f);
        yield return null;

        while (AnimHelper.IsPlaying(animator, "FadeIn"))
        {
            yield return null;
        }

        textToUpdate.text = "1";
        animator.Play("FadeIn", 0, 0f);
        yield return null;

        while (AnimHelper.IsPlaying(animator, "FadeIn"))
        {
            yield return null;
        }

        textToUpdate.text = "Loop!";
        animator.Play("FadeIn", 0, 0f);
        yield return null;

        while (AnimHelper.IsPlaying(animator, "FadeIn"))
        {
            yield return null;
        }

        textToUpdate.text = "";
        game.BeginGame();
        yield return new WaitForSeconds(1f);

        textToUpdate.text = player.GetScoreBoardDisplayText();
        animator.Play("FadeInWithControls", 0, 0f);
        yield return null;

        while (AnimHelper.IsPlaying(animator, "FadeInWitControls"))
        {
            yield return null;
        }
    }

    public void GameOver()
    {
        StartCoroutine(IEGameOver());
    }

    private IEnumerator IEGameOver()
    {
        Animator animator = GetComponent<Animator>();
        animator.Play("FadeOutWithControls", 0, 0f);
        yield return null;

        while (AnimHelper.IsPlaying(animator, "FadeIn"))
        {
            Debug.Log("Quitting Countdown");
            yield return null;
        }
    }

}
