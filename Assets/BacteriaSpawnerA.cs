using UnityEngine;

public class BacteriaSpawnerA : MonoBehaviour
{
    public GameObject bacteria;
    public bool summon = false;

    void Update()
    {
        if (summon)
        {
            summon = false;

            GameObject clone = Instantiate(bacteria, transform.position, Quaternion.identity);

            // 3. Force the clone's script switch to be false so it doesn't instantly spawn a triplet
            BacteriaSpawnerA cloneScript = clone.GetComponent<BacteriaSpawnerA>();
            if (cloneScript != null)
            {
                cloneScript.summon = false;
            }
        }
    }
}