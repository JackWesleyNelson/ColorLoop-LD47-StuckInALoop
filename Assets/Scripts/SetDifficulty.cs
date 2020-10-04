using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDifficulty : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI difficultyText = null;
    [SerializeField]
    Button decrease = null;
    [SerializeField]
    Button increase = null;
    [SerializeField]
    Game game = null;

    private void Start()
    {
        SetDifficultyText();
        decrease.onClick.AddListener(DecreaseDifficulty);
        increase.onClick.AddListener(IncreaseDifficulty);

        decrease.onClick.AddListener(CheckButtonState);
        increase.onClick.AddListener(CheckButtonState);
        CheckButtonState();
    }

    private void SetDifficultyText()
    {
        switch (game.difficulty)
        {
            case Difficulty.VERY_EASY:
                difficultyText.text = "Very Easy";
                break;
            case Difficulty.EASY:
                difficultyText.text = "Easy";
                break;
            case Difficulty.MEDIUM:
                difficultyText.text = "Medium";
                break;
            case Difficulty.HARD:
                difficultyText.text = "Hard";
                break;
            case Difficulty.EXTREME:
                difficultyText.text = "Very Hard";
                break;
        }
    }

    private void DecreaseDifficulty()
    {
        if((int)game.difficulty > 0)
        {
            game.difficulty--;
        }
        SetDifficultyText();
    }

    private void IncreaseDifficulty()
    {
        int length = Enum.GetNames(typeof(Difficulty)).Length;
        if((int)game.difficulty < length-1)
        {
            game.difficulty++;
        }
        SetDifficultyText();
    }

    private void CheckButtonState()
    {
        if (game.difficulty == 0)
        {
            decrease.gameObject.SetActive(false);
        }
        else
        {
            decrease.gameObject.SetActive(true);
        }
        int length = Enum.GetNames(typeof(Difficulty)).Length;
        if ((int)game.difficulty < length - 1)
        {
            increase.gameObject.SetActive(true);
        }
        else
        {
            increase.gameObject.SetActive(false);
        }
    }

}