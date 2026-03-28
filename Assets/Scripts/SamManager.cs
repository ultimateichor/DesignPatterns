using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SamManager : MonoBehaviour
{
    [Header("References")]
    public SimonController simonController;
    public Image samVisualImage;
    public Image jumpscareImage;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip appearClip;
    public AudioClip deathClip;

    [Header("Difficulty")]
    public float minSpawnDelay = 10f;
    public float maxSpawnDelay = 20f;
    public int minPatternLength = 4;
    public int maxPatternLength = 6;

    private bool samActive = false;

    private void Start()
    {
        if (samVisualImage != null)
            samVisualImage.enabled = false;

        if (jumpscareImage != null)
            jumpscareImage.enabled = false;

        simonController.OnSuccess += HandleSuccess;
        simonController.OnFailure += HandleFailure;

        StartCoroutine(SamLoop());
    }

    private IEnumerator SamLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(waitTime);

            if (!samActive)
                StartCoroutine(SamAttack());
        }
    }

    private IEnumerator SamAttack()
    {
        samActive = true;

        if (samVisualImage != null)
            samVisualImage.enabled = true;

        if (audioSource != null && appearClip != null)
            audioSource.PlayOneShot(appearClip);

        yield return new WaitForSeconds(appearClip.length);

        List<SimonButton.ButtonColor> pattern = GeneratePattern();
        simonController.StartSequence(pattern);
    }

    private List<SimonButton.ButtonColor> GeneratePattern()
    {
        List<SimonButton.ButtonColor> pattern = new List<SimonButton.ButtonColor>();
        int length = Random.Range(minPatternLength, maxPatternLength + 1);

        for (int i = 0; i < length; i++)
            pattern.Add((SimonButton.ButtonColor)Random.Range(0, 4));

        return pattern;
    }

    private void HandleSuccess()
    {
        samActive = false;

        if (samVisualImage != null)
            samVisualImage.enabled = false;
    }

    private void HandleFailure()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        samActive = false;

        if (samVisualImage != null)
            samVisualImage.enabled = false;

        if (jumpscareImage != null)
            jumpscareImage.enabled = true;

        if (audioSource != null && deathClip != null)
            audioSource.PlayOneShot(deathClip);

        yield return new WaitForSeconds(deathClip != null ? deathClip.length : 1.5f);

        SceneManager.LoadScene(2);
    }
}
