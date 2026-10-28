using UnityEngine;

public class PacStudentCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pellet"))
        {
            GameManager.instance.AddScore(10);
            GameManager.instance.PelletEaten();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("PowerPellet"))
        {
            GameManager.instance.ActivatePowerPellet();
            GameManager.instance.PelletEaten();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Cherry"))
        {
            GameManager.instance.AddScore(100);
            Destroy(other.gameObject);

            CherryController cherry = FindAnyObjectByType<CherryController>();
            if (cherry != null) cherry.CherryEaten();
        }
        else if (other.CompareTag("Ghost"))
        {
            Debug.Log("1. PacStudent touched something tagged Ghost!");

            GhostController ghost = other.GetComponentInParent<GhostController>();

            if (ghost != null)
            {
                Debug.Log("2. Found GhostController! Ghost state is: " + ghost.currentState);

                if (ghost.currentState == GhostController.GhostState.Normal)
                {
                    Debug.Log("3. Ghost is Normal. Killing Player.");
                    GameManager.instance.PacStudentDied();
                }
                else if (ghost.currentState == GhostController.GhostState.Scared || ghost.currentState == GhostController.GhostState.Recovering)
                {
                    Debug.Log("3. Ghost is Scared. Eating Ghost.");
                    GameManager.instance.EatGhost(ghost);
                }
            }
            else
            {
                Debug.LogError("ERROR: Touched a 'Ghost', but the GhostController script is missing from that object or its parents!");
            }
        }
    }
}