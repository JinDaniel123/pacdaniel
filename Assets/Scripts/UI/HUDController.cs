using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
    public void ExitToMenu()
    {
        SceneManager.LoadScene("StartScene");
    }
}