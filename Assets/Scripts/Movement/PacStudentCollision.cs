using UnityEngine;

public class PacStudentCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pellet"))
        {
            GameManager.instance.AddScore(10);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("PowerPellet"))
        {
            GameManager.instance.ActivatePowerPellet();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Cherry"))
        {
            GameManager.instance.AddScore(100);
            Destroy(other.gameObject);
            FindFirstObjectByType<CherryController>().CherryEaten();
        }
        else if (other.CompareTag("Ghost"))
        {
            GhostController ghost = other.GetComponent<GhostController>();
            if (ghost.currentState == GhostController.GhostState.Scared || ghost.currentState == GhostController.GhostState.Recovering)
            {
                GameManager.instance.EatGhost(ghost);
            }
            else if (ghost.currentState == GhostController.GhostState.Normal)
            {
                GameManager.instance.PacStudentDied();
            }
        }
    }
}