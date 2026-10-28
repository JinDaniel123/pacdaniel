using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // 5 tiles per second
    private float lerpDuration;

    [Header("State Variables")]
    private Vector2 lastInput = Vector2.zero;
    private Vector2 currentInput = Vector2.zero;

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float lerpTimer = 0f;
    private bool isLerping = false;

    [Header("Visuals & Audio")]
    public Animator animator;
    public AudioSource movementAudio;
    public ParticleSystem dustParticles;

    [Header("Level Collision")]
    public LayerMask wallLayer;

    void Start()
    {
        lerpDuration = 1f / moveSpeed;

        lastInput = Vector2.right;
        currentInput = Vector2.right;

        startPosition = transform.position;
        targetPosition = transform.position;
    }

    void Update()
    {
        GatherInput();

        if (!isLerping)
        {
            DetermineMovement();
        }
        else
        {
            lerpTimer += Time.deltaTime;
            float percent = lerpTimer / lerpDuration;
            percent = Mathf.Clamp01(percent);

            transform.position = Vector2.Lerp(startPosition, targetPosition, percent);

            if (lerpTimer >= lerpDuration)
            {
                isLerping = false;
                transform.position = targetPosition;
                StopMovingEffects();
            }
        }
    }

    private void GatherInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) lastInput = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S)) lastInput = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A)) lastInput = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D)) lastInput = Vector2.right;
    }

    private void DetermineMovement()
    {
        Vector2 nextPosFromLastInput = (Vector2)transform.position + lastInput;

        if (IsWalkable(nextPosFromLastInput))
        {
            currentInput = lastInput;
            StartMoving();
            return;
        }

        Vector2 nextPosFromCurrentInput = (Vector2)transform.position + currentInput;

        if (IsWalkable(nextPosFromCurrentInput))
        {
            StartMoving();
            return;
        }

        StopMovingEffects();
    }

    private void StartMoving()
    {
        isLerping = true;
        lerpTimer = 0f;

        startPosition = transform.position;
        targetPosition = startPosition + currentInput;

        PlayMovingEffects();
    }

    private bool IsWalkable(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.2f, wallLayer);
        return hit == null;
    }

    private void PlayMovingEffects()
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
            animator.SetFloat("MoveX", currentInput.x);
            animator.SetFloat("MoveY", currentInput.y);
        }
        if (movementAudio != null && !movementAudio.isPlaying) movementAudio.Play();
        if (dustParticles != null && !dustParticles.isPlaying) dustParticles.Play();
    }

    private void StopMovingEffects()
    {
        if (animator != null) animator.SetBool("IsMoving", false);
        if (movementAudio != null) movementAudio.Stop();
        if (dustParticles != null) dustParticles.Stop();
    }
}