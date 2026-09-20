/*
using UnityEngine;

public class RBCSpawner_B : MonoBehaviour
{
    public GameObject rbc_b;
    public float interval = 0f;

    void Start()
    {
        for (int i = 0; i < 1; i++)
        {
            Vector3 spawnPos = new Vector3(10, 1, -10);
            Instantiate(rbc_b, spawnPos, Quaternion.identity);
        }
    }

    void Update()
    {
        interval += Time.deltaTime;
        if (interval >= 3f)
        {
            for (int i = 0; i < 1; i++)
            {
                Vector3 spawnPos = new Vector3(10, 1, -10);
                Instantiate(rbc_b, spawnPos, Quaternion.identity);
            }
            interval = 0f;
        }
    }
}
*/
using UnityEngine;

public class RBCSpawner_B : MonoBehaviour
{
    public GameObject RBC;
    public float interval = 0f;
    private int spawnCount = 0;
    private const int maxSpawns = 14;

    void Start()
    {
        Vector3 spawnPos = new Vector3(10, 1, -10);
        Instantiate(RBC, spawnPos, Quaternion.identity);
        spawnCount++;
    }

    void Update()
    {
        interval += Time.deltaTime;
        if (interval >= 3f && spawnCount < maxSpawns)
        {
            Vector3 spawnPos = new Vector3(10, 1, -10);
            Instantiate(RBC, spawnPos, Quaternion.identity);
            spawnCount++;
            interval = 0f;
        }
    }
}