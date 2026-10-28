using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ghostTimerText;
    public GameObject[] lifeIcons;

    [Header("Player Reference")]
    public PacStudentController pacStudent;
    public GameObject pacStudentDeathParticles;

    [Header("Audio References")]
    public AudioManagerController audioManager;

    [Header("Game Settings")]
    public int totalLives = 3;
    public int currentScore = 0;

    public bool isGamePaused = false;
    public float currentScaredTimer = 0f;

    private int currentLives;
    private Vector3 pacStudentStartPos;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        if (audioManager == null)
        {
            audioManager = FindFirstObjectByType<AudioManagerController>();
        }

        currentLives = totalLives;
        pacStudentStartPos = pacStudent.transform.position;
        UpdateScoreUI();
        UpdateLivesUI();
        ghostTimerText.gameObject.SetActive(false);
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + currentScore.ToString("D6");
    }

    void UpdateLivesUI()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].SetActive(i < currentLives);
        }
    }

    public void ActivatePowerPellet()
    {
        AddScore(50);
        StartCoroutine(PowerPelletRoutine());
    }

    IEnumerator PowerPelletRoutine()
    {
        currentScaredTimer = 10f;
        ghostTimerText.gameObject.SetActive(true);
        if (audioManager != null) audioManager.PlayScaredMusic();

        GhostController[] ghosts = FindObjectsByType<GhostController>(FindObjectsSortMode.None);
        foreach (GhostController ghost in ghosts) ghost.SetState(GhostController.GhostState.Scared);

        while (currentScaredTimer > 0)
        {
            currentScaredTimer -= Time.deltaTime;
            ghostTimerText.text = Mathf.CeilToInt(currentScaredTimer).ToString();

            if (currentScaredTimer <= 3f)
            {
                foreach (GhostController ghost in ghosts)
                {
                    if (ghost.currentState == GhostController.GhostState.Scared)
                        ghost.SetState(GhostController.GhostState.Recovering);
                }
            }
            yield return null;
        }

        foreach (GhostController ghost in ghosts)
        {
            if (ghost.currentState != GhostController.GhostState.Dead)
                ghost.SetState(GhostController.GhostState.Normal);
        }

        ghostTimerText.gameObject.SetActive(false);
        if (audioManager != null) audioManager.PlayNormalMusic();
    }

    public void EatGhost(GhostController ghost)
    {
        AddScore(300);
        if (audioManager != null) audioManager.PlayGhostEatSound();
        ghost.SetState(GhostController.GhostState.Dead);
    }

    public void PacStudentDied()
    {
        if (isGamePaused) return;
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        isGamePaused = true;
        currentLives--;
        UpdateLivesUI();

        pacStudent.animator.SetBool("IsDead", true);
        if (pacStudentDeathParticles != null) pacStudentDeathParticles.SetActive(true);
        if (audioManager != null) audioManager.PlayDeathSound();

        yield return new WaitForSeconds(1.5f);

        if (currentLives <= 0)
        {
            SceneManager.LoadScene("StartScene");
        }
        else
        {
            pacStudent.transform.position = pacStudentStartPos;
            pacStudent.animator.SetBool("IsDead", false);
            pacStudent.ResetMovement();
            if (pacStudentDeathParticles != null) pacStudentDeathParticles.SetActive(false);

            GhostController[] ghosts = FindObjectsByType<GhostController>(FindObjectsSortMode.None);
            foreach (GhostController ghost in ghosts) ghost.ResetGhost();

            isGamePaused = false;
        }
    }
}