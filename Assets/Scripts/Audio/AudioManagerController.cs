using UnityEngine;
using System.Collections;

public class AudioManagerController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip Intro;
    public AudioClip GhostNormal;

    void Start()
    {
        StartCoroutine(PlayIntroThenLoop());
    }

    IEnumerator PlayIntroThenLoop()
    {
        audioSource.clip = Intro;
        audioSource.loop = false;
        audioSource.Play();

        yield return new WaitForSeconds(Mathf.Min(Intro.length, 30f));

        audioSource.clip = GhostNormal;
        audioSource.loop = true;
        audioSource.Play();
    }
}