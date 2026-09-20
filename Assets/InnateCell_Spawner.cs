using UnityEngine;

public class InnateCell_Spawner : MonoBehaviour
{
    public GameObject macrophage;
    public GameObject neutrophil;

    public void SpawnMacrophage()
    {
        //Vector3 spawnPos = new Vector3(Random.Range(-13, 13), 1, Random.Range(-13, 13));
        Instantiate(macrophage, transform.position, Quaternion.identity);
    }

    public void SpawnNeutrophil()
    {
        //Vector3 spawnPos = new Vector3(Random.Range(-13, 13), 1, Random.Range(-13, 13));
        Instantiate(neutrophil, transform.position, Quaternion.identity);
    }
}