using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// Repeatedly spawns respiratory air batches and provides the Cough action.
/// </summary>
public sealed class RespiratorySystemController : MonoBehaviour
{
    private const int AirPerBatch = 5;
    private const float SpawnIntervalSeconds = 0.5f;
    private const float ContaminatedBatchChance = 0.05f;
    private const float CoughCooldownSeconds = 5f;
    private const float NavMeshSampleRadius = 50f;
    private const float ArrivalDistance = 1f;

    [SerializeField] private Transform airwaySpawnPoint;
    [SerializeField] private Transform leftLung;
    [SerializeField] private Transform rightLung;
    [SerializeField] private GameObject airPrefab;
    [SerializeField] private GameObject contaminatedAirPrefab;
    [SerializeField] private Button coughButton;
    [SerializeField] private TextMeshProUGUI coughButtonLabel;

    private float coughReadyAt;

    private void Start()
    {
        if (coughButton != null)
            coughButton.onClick.AddListener(HandleCoughPressed);
        else
            Debug.LogError("RespiratorySystemController requires a Cough button.", this);

        if (airwaySpawnPoint == null || leftLung == null || rightLung == null)
        {
            Debug.LogError("RespiratorySystemController requires Airway, Left Lung, and Right Lung points.", this);
            return;
        }

        if (airPrefab == null || contaminatedAirPrefab == null)
        {
            Debug.LogError("RespiratorySystemController requires both air prefabs.", this);
            return;
        }

        StartCoroutine(RunBatchLoop());
        RefreshCoughButton();
    }

    private void OnDestroy()
    {
        if (coughButton != null)
            coughButton.onClick.RemoveListener(HandleCoughPressed);
    }

    private void Update()
    {
        RefreshCoughButton();
    }

    private IEnumerator RunBatchLoop()
    {
        while (true)
        {
            GameObject selectedPrefab = UnityEngine.Random.value < ContaminatedBatchChance
                ? contaminatedAirPrefab
                : airPrefab;

            for (int index = 0; index < AirPerBatch; index++)
            {
                SpawnAir(selectedPrefab);
                if (index < AirPerBatch - 1)
                    yield return new WaitForSeconds(SpawnIntervalSeconds);
            }

            while (GetComponentsInChildren<RespiratoryAirAgent>(true).Length > 0)
                yield return null;
        }
    }

    private void SpawnAir(GameObject prefab)
    {
        NavMeshAgent prefabAgent = prefab.GetComponent<NavMeshAgent>();
        if (prefabAgent == null)
        {
            Debug.LogError($"Air prefab '{prefab.name}' requires a NavMeshAgent.", prefab);
            return;
        }

        NavMeshQueryFilter filter = new NavMeshQueryFilter
        {
            agentTypeID = prefabAgent.agentTypeID,
            areaMask = NavMesh.AllAreas
        };

        Transform lungPoint = UnityEngine.Random.value < 0.5f ? leftLung : rightLung;
        if (!NavMesh.SamplePosition(airwaySpawnPoint.position, out NavMeshHit airwayHit, NavMeshSampleRadius, filter))
        {
            Debug.LogWarning("No Respiratory System NavMesh position was found near Airway.", this);
            return;
        }

        if (!NavMesh.SamplePosition(lungPoint.position, out NavMeshHit lungHit, NavMeshSampleRadius, filter))
        {
            Debug.LogWarning($"No Respiratory System NavMesh position was found near {lungPoint.name}.", this);
            return;
        }

        GameObject air = Instantiate(prefab, airwayHit.position, airwaySpawnPoint.rotation, transform);
        NavMeshAgent agent = air.GetComponent<NavMeshAgent>();
        if (agent == null || (!agent.isOnNavMesh && !agent.Warp(airwayHit.position)))
        {
            Debug.LogWarning($"Spawned air '{air.name}' could not attach to the Respiratory System NavMesh.", air);
            Destroy(air);
            return;
        }

        RespiratoryAirAgent airAgent = air.GetComponent<RespiratoryAirAgent>();
        if (airAgent == null)
            airAgent = air.AddComponent<RespiratoryAirAgent>();

        if (!airAgent.Initialize(agent, lungHit.position, airwayHit.position, ArrivalDistance))
        {
            Debug.LogWarning($"Spawned air '{air.name}' could not start its route.", air);
            Destroy(air);
        }
    }

    private void HandleCoughPressed()
    {
        if (coughButton == null || !coughButton.interactable)
            return;

        coughReadyAt = Time.time + CoughCooldownSeconds;
        RespiratoryAirAgent[] airInstances = GetComponentsInChildren<RespiratoryAirAgent>(true);
        foreach (RespiratoryAirAgent airInstance in airInstances)
        {
            if (airInstance != null)
                Destroy(airInstance.gameObject);
        }

        RefreshCoughButton();
    }

    private void RefreshCoughButton()
    {
        if (coughButton == null)
            return;

        float remainingCooldown = coughReadyAt - Time.time;
        bool isOnCooldown = remainingCooldown > 0f;
        coughButton.interactable = !isOnCooldown;
        if (coughButtonLabel != null)
            coughButtonLabel.text = isOnCooldown ? $"COUGH ({Mathf.CeilToInt(remainingCooldown)}s)" : "COUGH";
    }
}
