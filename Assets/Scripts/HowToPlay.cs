using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlay : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu = null;
    [SerializeField]
    private GameObject howToPlay = null;

    public void StartHowToPlay()
    {
        mainMenu.SetActive(false);
        howToPlay.SetActive(true);
    }
    public void ExitHowToPlay()
    {
        mainMenu.SetActive(true);
        howToPlay.SetActive(false);
    }

    private void Update()
    {
        foreach(KeyCode k in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(k))
            {
                ExitHowToPlay();
            }
        }
    }

}
