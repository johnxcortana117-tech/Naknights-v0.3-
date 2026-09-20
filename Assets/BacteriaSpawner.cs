using UnityEngine;

public class BacteriaSpawner : MonoBehaviour
{
    public GameObject bacteria;

    public float timer = 0f;
    public float SpawnInterval = 30f;
    public int SpawnCount = 3;

    void Start()
    {
        for (int i = 1; i < SpawnCount; i++)
        {
            GameObject clone = Instantiate(bacteria, transform.position, Quaternion.identity);

            // Turn off the timer/script on the clone if you want to prevent it from multiplying
            BacteriaSpawnerA cloneScript = clone.GetComponent<BacteriaSpawnerA>();
            if (cloneScript != null)
            {
                cloneScript.enabled = false;
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= SpawnInterval)
        {
            timer = 0f;

            for (int i = 1; i < SpawnCount; i++)
            {
                GameObject clone = Instantiate(bacteria, transform.position, Quaternion.identity);

                BacteriaSpawnerA cloneScript = clone.GetComponent<BacteriaSpawnerA>();
                if (cloneScript != null)
                {
                    cloneScript.enabled = false;
                }
            }
        }
    }
}