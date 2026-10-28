using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; // Required to disable buttons

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ghostTimerText;
    public TextMeshProUGUI gameTimerText;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI gameOverText;
    public GameObject blockerImage;
    public GameObject[] lifeIcons;

    [Header("Player Reference")]
    public PacStudentController pacStudent;
    public GameObject pacStudentDeathParticles;

    [Header("Audio References")]
    public AudioManagerController audioManager;

    [Header("Game Settings")]
    public int totalLives = 3;
    public int currentScore = 0;

    public bool isGamePaused = true;
    public float currentScaredTimer = 0f;
    public float gameTime = 0f;

    private bool isRoundStarted = false;
    private int currentLives;
    private int pelletsRemaining;
    private Vector3 pacStudentStartPos;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        if (audioManager == null) audioManager = FindAnyObjectByType<AudioManagerController>();
        if (pacStudent == null) pacStudent = FindAnyObjectByType<PacStudentController>();
        if (pacStudent != null) pacStudentStartPos = pacStudent.transform.position;

        currentLives = totalLives;

        UpdateScoreUI();
        UpdateLivesUI();
        UpdateTimerUI();

        if (ghostTimerText != null) ghostTimerText.gameObject.SetActive(false);
        if (gameOverText != null) gameOverText.gameObject.SetActive(false);
        if (blockerImage != null) blockerImage.SetActive(false);

        pelletsRemaining = GameObject.FindGameObjectsWithTag("Pellet").Length + GameObject.FindGameObjectsWithTag("PowerPellet").Length;

        StartCoroutine(RoundStartRoutine());
    }

    void Update()
    {
        if (isRoundStarted && !isGamePaused)
        {
            gameTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        if (gameTimerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60F);
            int seconds = Mathf.FloorToInt(gameTime - minutes * 60f);
            int milliseconds = Mathf.FloorToInt((gameTime - Mathf.FloorToInt(gameTime)) * 1000f);
            gameTimerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }

    IEnumerator RoundStartRoutine()
    {
        isGamePaused = true;
        if (blockerImage != null) blockerImage.SetActive(true);
        if (countdownText != null) countdownText.gameObject.SetActive(true);

        if (audioManager != null) audioManager.PlayIntroMusic();

        if (countdownText != null) countdownText.text = "3";
        yield return new WaitForSeconds(1f);
        if (countdownText != null) countdownText.text = "2";
        yield return new WaitForSeconds(1f);
        if (countdownText != null) countdownText.text = "1";
        yield return new WaitForSeconds(1f);
        if (countdownText != null) countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        if (countdownText != null) countdownText.gameObject.SetActive(false);
        if (blockerImage != null) blockerImage.SetActive(false);

        isRoundStarted = true;
        isGamePaused = false;

        if (audioManager != null) audioManager.PlayNormalMusic();
    }

    public void PelletEaten()
    {
        pelletsRemaining--;
        if (pelletsRemaining <= 0)
        {
            TriggerGameOver();
        }
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
            if (lifeIcons[i] != null)
            {
                lifeIcons[i].SetActive(i < currentLives);
            }
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
        if (ghostTimerText != null) ghostTimerText.gameObject.SetActive(true);
        if (audioManager != null) audioManager.PlayScaredMusic();

        GhostController[] ghosts = FindObjectsByType<GhostController>(FindObjectsInactive.Exclude);
        foreach (GhostController ghost in ghosts) ghost.SetState(GhostController.GhostState.Scared);

        while (currentScaredTimer > 0)
        {
            currentScaredTimer -= Time.deltaTime;
            if (ghostTimerText != null) ghostTimerText.text = Mathf.CeilToInt(currentScaredTimer).ToString();

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

        if (ghostTimerText != null) ghostTimerText.gameObject.SetActive(false);
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
        isGamePaused = true;
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        currentLives--;
        UpdateLivesUI();

        if (pacStudent != null && pacStudent.animator != null) pacStudent.animator.SetBool("IsDead", true);
        if (pacStudentDeathParticles != null) pacStudentDeathParticles.SetActive(true);
        if (audioManager != null) audioManager.PlayDeathSound();

        yield return new WaitForSeconds(1.5f);

        if (currentLives <= 0)
        {
            TriggerGameOver();
        }
        else
        {
            if (pacStudent != null)
            {
                pacStudent.transform.position = pacStudentStartPos;
                if (pacStudent.animator != null) pacStudent.animator.SetBool("IsDead", false);
                pacStudent.ResetMovement();
            }
            if (pacStudentDeathParticles != null) pacStudentDeathParticles.SetActive(false);

            GhostController[] ghosts = FindObjectsByType<GhostController>(FindObjectsInactive.Exclude);
            foreach (GhostController ghost in ghosts) ghost.ResetGhost();

            isGamePaused = false;
        }
    }

    void TriggerGameOver()
    {
        if (isGamePaused && blockerImage.activeSelf) return;
        isGamePaused = true;

        // 1. Show UI
        if (blockerImage != null) blockerImage.SetActive(true);
        if (gameOverText != null) gameOverText.gameObject.SetActive(true);

        // 2. Disable all UI buttons
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Exclude);
        foreach (Button btn in allButtons) btn.interactable = false;

        // 3. Stop Audio
        if (audioManager != null) audioManager.StopMusic();

        // 4. Save PlayerPrefs
        int savedScore = PlayerPrefs.GetInt("HighScore1", 0);
        float savedTime = PlayerPrefs.GetFloat("HighTime1", 999999f);

        // If current score is higher, OR score is the same but time is lower
        if (currentScore > savedScore || (currentScore == savedScore && gameTime < savedTime))
        {
            PlayerPrefs.SetInt("HighScore1", currentScore);
            PlayerPrefs.SetFloat("HighTime1", gameTime);
            PlayerPrefs.Save();
        }

        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("StartScene");
    }
}