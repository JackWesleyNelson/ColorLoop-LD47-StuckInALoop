using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Quit : MonoBehaviour
{

    public void QuitButton()
    {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #endif
    }

}
