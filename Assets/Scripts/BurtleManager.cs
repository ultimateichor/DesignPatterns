using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BurtleManager : MonoBehaviour
{
    [Header("Order Display")]
    public Image[] orderSlots;
    public Sprite[] ingredientSprites;

    [Header("UI")]
    public Image patienceBar;
    public TextMeshProUGUI orderCounter;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip newOrderClip;
    public AudioClip orderCompleteClip;
    public AudioClip buttonClickClip;

    [Header("Settings")]
    public float patienceDrainRate = 0.008f;   // drains fully in ~125 seconds
    public float patienceRefillAmount = 0.3f;   // each completed order refills 30%
    public float startDelay = 10f;

    private int nightGoal;
    private int ordersCompleted = 0;
    private int[] currentOrder;
    private int currentIngredientIndex = 0;
    private float patience = 1f;
    private bool gameStarted = false;
    private bool isActive = false;
    private bool won = false;
    private bool firstOrder = true;

    void Start()
    {
        nightGoal = 67;
        UpdateCounter();
        patienceBar.fillAmount = 1f;
        Invoke(nameof(StartNewOrder), startDelay);
    }

    void Update()
    {
        if (won || !gameStarted) return;

        patience -= patienceDrainRate * Time.deltaTime;
        patience = Mathf.Clamp01(patience);
        patienceBar.fillAmount = patience;

        if (patience <= 0f)
            Die();
    }

    void StartNewOrder()
    {
        gameStarted = true;

        int middleCount = Random.Range(1, 4);
        currentOrder = new int[middleCount + 2];
        currentOrder[0] = 0;
        for (int i = 0; i < middleCount; i++)
            currentOrder[i + 1] = Random.Range(1, 5);
        currentOrder[currentOrder.Length - 1] = 0;

        currentIngredientIndex = 0;

        for (int i = 0; i < orderSlots.Length; i++)
        {
            if (i < currentOrder.Length)
            {
                orderSlots[i].sprite = ingredientSprites[currentOrder[i]];
                orderSlots[i].enabled = true;
            }
            else
            {
                orderSlots[i].enabled = false;
            }
        }

        isActive = true;

        if (firstOrder && audioSource != null && newOrderClip != null)
        {
            audioSource.PlayOneShot(newOrderClip);
            firstOrder = false;
        }
    }

    public void OnIngredientClicked(int ingredientIndex)
    {
        if (!isActive) return;

        if (ingredientIndex == currentOrder[currentIngredientIndex])
        {
            if (audioSource != null && buttonClickClip != null)
                audioSource.PlayOneShot(buttonClickClip);

            currentIngredientIndex++;
            if (currentIngredientIndex >= currentOrder.Length)
            {
                ordersCompleted++;
                UpdateCounter();
                isActive = false;

                foreach (var slot in orderSlots)
                    slot.enabled = false;

                patience = Mathf.Min(1f, patience + patienceRefillAmount);
                patienceBar.fillAmount = patience;

                if (audioSource != null && orderCompleteClip != null)
                    audioSource.PlayOneShot(orderCompleteClip);

                if (ordersCompleted >= nightGoal)
                {
                    won = true;
                    SceneManager.LoadScene(3);
                }
                else
                {
                    StartNewOrder();
                }
            }
        }
        else
        {
            Die();
        }
    }

    void UpdateCounter()
    {
        orderCounter.text = ordersCompleted + " / " + nightGoal;
    }

    void Die()
    {
        SceneManager.LoadScene(2);
    }
}
