using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Death Screen UI")]
    public GameObject deathScreen;         // Full-screen overlay panel
    public TextMeshProUGUI deathMessage;   // Optional flavour text on the overlay

    private bool isDead;

    public bool IsDead => isDead;

    void Awake()
    {
        // Singleton — only one GameManager allowed
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    // ── Called by any animatronic or hazard that kills the player ─────────
    public void PlayerDie()
    {
        if (isDead) return;
        isDead = true;

        Time.timeScale = 0f;   // Freeze everything

        if (deathScreen != null)
            deathScreen.SetActive(true);

        if (deathMessage != null)
            deathMessage.text = "YOU DIED";
    }

    // ── Hook this to your "Try Again" button's OnClick ────────────────────
    public void RestartGame()
    {
        isDead = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
