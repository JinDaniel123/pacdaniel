using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public AudioManagerController audioManager;

    public void LoadLevel1()
    {
        Debug.Log("BUTTON CLICKED! Attempting to load scene...");

        if (audioManager != null)
        {
            audioManager.StartGameMusic();
        }

        SceneManager.LoadScene("Level01");
    }

    public void LoadLevel2()
    {
        if (audioManager != null)
        {
            audioManager.StartGameMusic(); 
        }
        SceneManager.LoadScene("Level02");
    }
}