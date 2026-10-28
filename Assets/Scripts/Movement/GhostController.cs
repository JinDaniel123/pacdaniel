using UnityEngine;

public class GhostController : MonoBehaviour
{
    public enum GhostState { Normal, Scared, Recovering, Dead }
    public GhostState currentState = GhostState.Normal;

    public Animator animator;
    public Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    public void SetState(GhostState newState)
    {
        if (currentState == GhostState.Dead && newState != GhostState.Dead) return;

        currentState = newState;

        if (animator != null)
        {
            if (newState == GhostState.Normal) animator.SetTrigger("Normal");
            else if (newState == GhostState.Scared) animator.SetTrigger("Scared");
            else if (newState == GhostState.Recovering) animator.SetTrigger("Recovering");
            else if (newState == GhostState.Dead)
            {
                animator.SetTrigger("Dead");
                StartCoroutine(RespawnRoutine());
            }
        }
    }

    System.Collections.IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(3f);

        if (GameManager.instance.currentScaredTimer > 3f) SetState(GhostState.Scared);
        else if (GameManager.instance.currentScaredTimer > 0) SetState(GhostState.Recovering);
        else SetState(GhostState.Normal);
    }

    public void ResetGhost()
    {
        transform.position = startPosition;
        SetState(GhostState.Normal);
    }

    void Update()
    {
        if (GameManager.instance.isGamePaused) return;
        Move();
    }

    void Move()
    {

    }
}