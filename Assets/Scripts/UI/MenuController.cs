using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuController : MonoBehaviour
{
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level101");
    }

    public void LoadLevel2()
    {
        
        SceneManager.LoadScene("Level02");
    }
}