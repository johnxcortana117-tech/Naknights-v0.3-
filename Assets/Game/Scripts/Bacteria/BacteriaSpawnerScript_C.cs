using UnityEngine;

public class BacteriaSpawnerScript_C : MonoBehaviour
{
    public GameObject bacteria;
    public float interval = 0f;

    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(13.5f, -13.5f), 1, Random.Range(116.5f, 123.5f));
            Instantiate(bacteria, spawnPos, Quaternion.identity);
        }
    }

    void Update()
    {
        interval += Time.deltaTime;
        if (interval >= 30f)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector3 spawnPos = new Vector3(Random.Range(13.5f, -13.5f), 1, Random.Range(116.5f, 123.5f));
                Instantiate(bacteria, spawnPos, Quaternion.identity);
            }
            interval = 0f;
        }
    }
}