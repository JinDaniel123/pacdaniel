using UnityEngine;
using System.Collections;

public class AudioManagerController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip MenuMusic;
    public AudioClip Intro;
    public AudioClip GhostNormal;

    public AudioClip GhostScared;
    public AudioClip GhostEatSound;
    public AudioClip DeathSound;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if (MenuMusic != null)
        {
            audioSource.clip = MenuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void StartGameMusic()
    {
        StartCoroutine(PlayIntroThenLoop());
    }

    IEnumerator PlayIntroThenLoop()
    {
        audioSource.Stop();
        audioSource.clip = Intro;
        audioSource.loop = false;
        audioSource.Play();

        yield return new WaitForSeconds(Mathf.Min(Intro.length, 30f));

        PlayNormalMusic();
    }

    public void PlayNormalMusic()
    {
        audioSource.Stop();
        audioSource.clip = GhostNormal;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayScaredMusic()
    {
        audioSource.Stop();
        audioSource.clip = GhostScared;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayGhostEatSound()
    {
        audioSource.PlayOneShot(GhostEatSound);
    }

    public void PlayDeathSound()
    {
        audioSource.PlayOneShot(DeathSound);
    }
}