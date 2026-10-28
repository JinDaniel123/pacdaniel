using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    private float lerpDuration;

    [Header("State Variables")]
    private Vector2 lastInput = Vector2.zero;
    private Vector2 currentInput = Vector2.zero;

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float lerpTimer = 0f;
    private bool isLerping = false;

    [Header("Teleporters")]
    public Transform leftTeleportPoint;
    public Transform rightTeleportPoint;

    [Header("Visuals & Audio")]
    public Animator animator;
    public AudioSource movementAudio;
    public AudioClip movingClip;
    public AudioClip eatingClip;
    public ParticleSystem dustParticles;

    [Header("Wall Collision Effects")]
    public ParticleSystem wallBumpParticles;
    public AudioSource wallBumpAudio;

    [Header("Level Collision")]
    public LayerMask wallLayer;
    public LayerMask pelletLayer;

    void Start()
    {
        if (leftTeleportPoint == null)
        {
            GameObject leftObj = GameObject.Find("LeftTeleportPoint");
            if (leftObj != null) leftTeleportPoint = leftObj.transform;
        }

        if (rightTeleportPoint == null)
        {
            GameObject rightObj = GameObject.Find("RightTeleportPoint");
            if (rightObj != null) rightTeleportPoint = rightObj.transform;
        }

        if (dustParticles == null)
        {
            GameObject dustObj = GameObject.Find("DustParticles");
            if (dustObj != null) dustParticles = dustObj.GetComponent<ParticleSystem>();
        }

        lerpDuration = 1f / moveSpeed;
        ResetMovement();
    }

    public void ResetMovement()
    {
        lastInput = Vector2.right;
        currentInput = Vector2.right;
        startPosition = transform.position;
        targetPosition = transform.position;
        isLerping = false;
        lerpTimer = 0f;
        StopMovingEffects();
    }

    void Update()
    {
        // If the game is paused (countdown or death), stop moving instantly
        if (GameManager.instance != null && GameManager.instance.isGamePaused)
        {
            StopMovingEffects();
            return;
        }

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

                if (leftTeleportPoint != null && rightTeleportPoint != null)
                {
                    if (transform.position.x <= leftTeleportPoint.position.x)
                    {
                        transform.position = new Vector2(rightTeleportPoint.position.x, transform.position.y);
                    }
                    else if (transform.position.x >= rightTeleportPoint.position.x)
                    {
                        transform.position = new Vector2(leftTeleportPoint.position.x, transform.position.y);
                    }
                }

                startPosition = transform.position;
                targetPosition = transform.position;
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

        if (currentInput != Vector2.zero)
        {
            PlayWallBumpEffect();
            currentInput = Vector2.zero;
            StopMovingEffects();
        }
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

        Collider2D pelletHit = Physics2D.OverlapCircle(targetPosition, 0.2f, pelletLayer);
        if (movementAudio != null)
        {
            if (pelletHit != null && eatingClip != null)
            {
                movementAudio.clip = eatingClip;
            }
            else if (movingClip != null)
            {
                movementAudio.clip = movingClip;
            }
            if (!movementAudio.isPlaying) movementAudio.Play();
        }

        if (dustParticles != null && !dustParticles.isPlaying) dustParticles.Play();
    }

    private void StopMovingEffects()
    {
        if (animator != null) animator.SetBool("IsMoving", false);
        if (movementAudio != null) movementAudio.Stop();
        if (dustParticles != null) dustParticles.Stop();
    }

    private void PlayWallBumpEffect()
    {
        if (wallBumpParticles != null) wallBumpParticles.Play();
        if (wallBumpAudio != null) wallBumpAudio.Play();
    }
}