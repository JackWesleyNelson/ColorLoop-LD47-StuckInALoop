using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInMainMusic : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource = null;

    private float volumeCap = .2f;
    private float volumeIncreaseRate = .01f;


    private void Start()
    {
        audioSource.volume = 0;
        StartCoroutine(RaiseVolume());
    }

    private IEnumerator RaiseVolume()
    {
        while(audioSource.volume < volumeCap)
        {
            audioSource.volume = Mathf.Min(audioSource.volume + volumeIncreaseRate, volumeCap);
            yield return null;
        }
    }

}
