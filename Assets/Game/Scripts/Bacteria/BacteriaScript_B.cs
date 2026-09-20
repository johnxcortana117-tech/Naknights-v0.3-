using UnityEngine;
using UnityEngine.AI;

public class BacteriaScript_B : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;

    public float bacteriaHP = 10f;
    public float radius = 1f;
    public float damage = 10f;

    void Start()
    {
        InvokeRepeating(nameof(AttackTissue), 1f, 1f);
    }

    void Update()
    {
        if (target == null)
        {
            Track();
        }

        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        else
        {
            agent.ResetPath();
        }
    }

    void Track()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Tissue B");
        float closestDist = Mathf.Infinity;
        Transform mainTarget = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDist)
            {
                closestDist = distance;
                mainTarget = enemy.transform;
            }
        }
        target = mainTarget;
    }

    void AttackTissue()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Tissue B"))
            {
                TissueHealthScript tissue = hit.GetComponent<TissueHealthScript>();
                if (tissue.tissueHP > 0)
                    tissue.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        bacteriaHP -= amount;

        if (bacteriaHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}