using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PulpitManager : MonoBehaviour
{
    public GameObject platformPrefab;
    public Transform player;

    private List<GameObject> platforms = new List<GameObject>();
    private float minLifetime;
    private float maxLifetime;
    private float spawnTime;
    private float platformOffset;

    private GameObject previousPlatform;
    private GameObject currentPlatform;
    private float timeSinceLastSpawn;

    private Vector3? lastPosition = null;
    private Vector3? secondLastPosition = null;
    public TextMeshProUGUI scoreText;
    public int score = 0;

    public GameStateManager gameStateManager;

    public void Init()
    {
        var config = ConfigurationManager.GetConfig();
        if (config != null)
        {
            minLifetime = config.pulpit_data.min_pulpit_destroy_time;
            maxLifetime = config.pulpit_data.max_pulpit_destroy_time;
            spawnTime = config.pulpit_data.pulpit_spawn_time;
            platformOffset = config.pulpit_data.pulpit_size;
        }
        else
        {
            Debug.LogWarning("Configuration data could not be loaded.");
        }

        //initial pulpit
        currentPlatform = SpawnPlatformUnderPlayer();
        platforms.Add(currentPlatform);
        previousPlatform = null;
        timeSinceLastSpawn = 0f;

        StartCoroutine(PlatformRoutine());
    }

    private IEnumerator PlatformRoutine()
    {
        //current pulpit
        float currentLifetime = Random.Range(minLifetime, maxLifetime);
        PlatformTimer platformTimer = currentPlatform.GetComponent<PlatformTimer>();
        if (platformTimer != null)
        {
            platformTimer.lifetime = currentLifetime;
        }

        yield return new WaitForSeconds(spawnTime);

        //second pulpit
        previousPlatform = currentPlatform;
        currentPlatform = SpawnPlatform();
        platforms.Add(currentPlatform);

        float remainingLifetime = platformTimer != null ? platformTimer.GetRemainingLifetime() : currentLifetime;
        float newLifetime;

        //Edge Case: new pulpit died faster than last pulpit
        do
        {
            newLifetime = Random.Range(minLifetime, maxLifetime);
        } while (newLifetime < remainingLifetime);

        PlatformTimer newPlatformTimer = currentPlatform.GetComponent<PlatformTimer>();
        if (newPlatformTimer != null)
        {
            newPlatformTimer.lifetime = newLifetime;
        }

        lastPosition = currentPlatform.transform.position;

        while (true)
        {
            if (player.position.y < -10f || score >= 50)
            {
                gameStateManager.GameOver();
                yield break;
            } 
            timeSinceLastSpawn += Time.deltaTime;
            if (previousPlatform != null && previousPlatform.GetComponent<PlatformTimer>().IsLifetimeExpired())
            {
                platforms.Remove(previousPlatform);
                // if (previousPlatform != null)
                // {
                //     StartCoroutine(PopOut(previousPlatform));
                //     new WaitForSeconds(0.5f);
                // }
                Destroy(previousPlatform);

                IncrementScore();

                previousPlatform = currentPlatform;
                currentPlatform = SpawnPlatform();
                platforms.Add(currentPlatform);

                remainingLifetime = newPlatformTimer != null ? newPlatformTimer.GetRemainingLifetime() : newLifetime;
                do
                {
                    newLifetime = Random.Range(minLifetime, maxLifetime);
                } while (newLifetime < remainingLifetime);

                newPlatformTimer = currentPlatform.GetComponent<PlatformTimer>();
                if (newPlatformTimer != null)
                {
                    newPlatformTimer.lifetime = newLifetime;
                }

                secondLastPosition = lastPosition;
                lastPosition = currentPlatform.transform.position;

                timeSinceLastSpawn = 0f;
            }

            yield return null;
        }
    }


    private GameObject SpawnPlatform()
    {
        Vector3 spawnPosition = Vector3.zero;

        if (previousPlatform != null)
        {
            Vector3 lastPlatformPosition = previousPlatform.transform.position;

            Vector3[] directions = {
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1)
            };

            Vector3 chosenDirection;
            bool validPositionFound = false;
            float minDistance = 1f;

            while (!validPositionFound)
            {
                chosenDirection = directions[Random.Range(0, directions.Length)];
                spawnPosition = lastPlatformPosition + chosenDirection * platformOffset;

                // Check if the new position is far enough from the last two positions
                bool isTooClose = false;
                if (lastPosition.HasValue && Vector3.Distance(spawnPosition, lastPosition.Value) < minDistance)
                {
                    isTooClose = true;
                }
                if (secondLastPosition.HasValue && Vector3.Distance(spawnPosition, secondLastPosition.Value) < minDistance)
                {
                    isTooClose = true;
                }

                if (!isTooClose)
                {
                    validPositionFound = true;
                }
            }

            lastPosition = previousPlatform.transform.position;
        }

        GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        if (platform)
        {
            StartCoroutine(PopIn(platform));
        }
        return platform;
    }

    private GameObject SpawnPlatformUnderPlayer()
    {
        Vector3 playerPosition = player.position;
        Vector3 spawnPosition = new Vector3(playerPosition.x, playerPosition.y - 1f, playerPosition.z);

        GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        return platform;
    }

    private IEnumerator PopIn(GameObject platform)
    {
        if (platform == null)
        {
            yield break;
        }

        Vector3 initialScale = new Vector3(0f, platform.transform.localScale.y, 0f);
        Vector3 targetScale = new Vector3(1f, platform.transform.localScale.y, 1f);
        float duration = 0.1f;
        float elapsedTime = 0f;

        // Set initial scale
        platform.transform.localScale = initialScale;

        // Define movement
        Vector3 initialPosition = new Vector3(platform.transform.position.x, platform.transform.position.y - 1, platform.transform.position.z);
        Vector3 targetPosition = platform.transform.position;

        // Animation loop
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            platform.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            platform.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final state
        platform.transform.localScale = targetScale;
        platform.transform.position = targetPosition;
    }

    // private IEnumerator PopOut(GameObject platform)
    // {
    //     if (platform == null)
    //     {
    //         yield break;
    //     }

    //     Vector3 initialScale = platform.transform.localScale;
    //     Vector3 targetScale = new Vector3(0f, platform.transform.localScale.y, 0f);
    //     float duration = 0.1f;
    //     float elapsedTime = 0f;

    //     // Define movement
    //     Vector3 initialPosition = platform.transform.position;
    //     Vector3 targetPosition = new Vector3(platform.transform.position.x, platform.transform.position.y - 1, platform.transform.position.z);

    //     // Animation loop
    //     while (elapsedTime < duration)
    //     {
    //         float t = elapsedTime / duration;
    //         platform.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
    //         platform.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     // Ensure final state
    //     platform.transform.localScale = targetScale;
    //     platform.transform.position = targetPosition;
    // }
    private void IncrementScore()
    {
        score++;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        Debug.Log(score.ToString());
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}