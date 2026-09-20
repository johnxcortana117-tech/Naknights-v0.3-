using UnityEngine;
using UnityEngine.AI;

public class RBC_3 : MonoBehaviour
{
    public NavMeshAgent agent;
    public Vector3 pointA;
    public Vector3 pointB;
    public Vector3 pointC;
    public Vector3 pointD;
    private int currentPointIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pointA = GameObject.FindGameObjectWithTag("3.1").transform.position;
        pointB = GameObject.FindGameObjectWithTag("3.2").transform.position;
        pointC = GameObject.FindGameObjectWithTag("3.3").transform.position;
        pointD = GameObject.FindGameObjectWithTag("Heart").transform.position;

        MoveToNextPoint();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            MoveToNextPoint();
        }
    }

    private void MoveToNextPoint()
    {
        switch (currentPointIndex)
        {
            case 0:
                agent.SetDestination(pointA);
                currentPointIndex = 1;
                break;
            case 1:
                agent.SetDestination(pointB);
                currentPointIndex = 2;
                break;
            case 2:
                agent.SetDestination(pointC);
                currentPointIndex = 3;
                break;
            case 3:
                agent.SetDestination(pointD);
                currentPointIndex = 0;
                break;
        }
    }
}