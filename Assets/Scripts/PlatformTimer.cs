using UnityEngine;
using TMPro;
using System.Collections;

public class PlatformTimer : MonoBehaviour
{
    public float lifetime;
    private TMP_Text timerText;
    private float timer;

    private void Start()
    {
        timerText = GetComponentInChildren<TMP_Text>();

        if (timerText == null)
        {
            Debug.LogError("No Text component found on the pulpit!");
        }

        StartCoroutine(UpdateTimer());
        timer = 0f;
    }

    private IEnumerator UpdateTimer()
    {
        while (timer < lifetime)
        {
            timer += Time.deltaTime;
            float remainingTime = lifetime - timer;
            timerText.text = remainingTime.ToString("F2") + "s";
            yield return null;
        }

        Destroy(gameObject);
    }

    public bool IsLifetimeExpired()
    {
        return timer >= lifetime;
    }

    public float GetRemainingLifetime()
    {
        return lifetime - timer;
    }
}