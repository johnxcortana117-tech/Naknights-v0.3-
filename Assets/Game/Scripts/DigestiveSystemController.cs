using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// Runs scheduled and player-triggered food batches for the digestive map.
/// </summary>
public sealed class DigestiveSystemController : MonoBehaviour
{
    private const int FoodPerBatch = 10;
    private const float SpawnIntervalSeconds = 0.5f;
    private const float ContaminatedBatchChance = 0.05f;
    private const float ManualEatCooldownSeconds = 5f;
    private const float ScheduledEatLockSeconds = 10f;
    private const float VomitCooldownSeconds = 15f;
    private const float NavMeshSampleRadius = 50f;
    private const float ArrivalDistance = 1f;
    private const float DigestiveBoundsPadding = 2f;
    private const string DestinationPointsPath = "Digestive System/Destionation Points";
    private const string OrdersContainerPath = "HUDCanvas/DigestiveOrdersContainer";

    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private GameObject contaminatedFoodPrefab;
    [SerializeField] private Transform digestivePlatform;
    [SerializeField] private DayCounterUI dayCounter;

    private Transform mouthPoint;
    private Transform rectumPoint;
    private Button eatButton;
    private Button vomitButton;
    private TextMeshProUGUI eatLabel;
    private TextMeshProUGUI vomitLabel;
    private Coroutine activeBatch;
    private float eatCooldownUntil;
    private float scheduledEatLockUntil;
    private float vomitCooldownUntil;
    private int lastScheduledDay = int.MinValue;
    private int lastScheduledHour = -1;
    private bool isBatchRunning;

    private void Start()
    {
        Transform destinationPoints = transform.Find(DestinationPointsPath);
        if (destinationPoints != null)
        {
            mouthPoint = destinationPoints.Find("Mouth");
            rectumPoint = destinationPoints.Find("Rectum");
        }

        if (dayCounter == null)
            dayCounter = FindFirstObjectByType<DayCounterUI>();

        GameObject ordersContainer = GameObject.Find(OrdersContainerPath);
        if (ordersContainer != null)
        {
            Transform buttons = ordersContainer.transform.Find("Buttons");
            Transform eatTransform = buttons != null ? buttons.Find("EatButton") : null;
            Transform vomitTransform = buttons != null ? buttons.Find("VomitButton") : null;
            if (eatTransform != null)
            {
                eatButton = eatTransform.GetComponent<Button>();
                eatLabel = eatTransform.GetComponentInChildren<TextMeshProUGUI>(true);
            }
            if (vomitTransform != null)
            {
                vomitButton = vomitTransform.GetComponent<Button>();
                vomitLabel = vomitTransform.GetComponentInChildren<TextMeshProUGUI>(true);
            }
        }

        if (dayCounter != null)
        {
            dayCounter.OnTimeChanged += HandleTimeChanged;
            HandleTimeChanged(dayCounter.CurrentHour, dayCounter.CurrentMinute);
        }
        else
        {
            Debug.LogError("DigestiveSystemController could not find the DayCounterUI.", this);
        }

        if (eatButton != null)
            eatButton.onClick.AddListener(HandleEatPressed);
        else
            Debug.LogError("Digestive orders container is missing its Eat button.", this);

        if (vomitButton != null)
            vomitButton.onClick.AddListener(HandleVomitPressed);
        else
            Debug.LogError("Digestive orders container is missing its Vomit button.", this);

        if (mouthPoint == null || rectumPoint == null)
            Debug.LogError("DigestiveSystemController requires Mouth and Rectum destination points.", this);
        if (foodPrefab == null || contaminatedFoodPrefab == null)
            Debug.LogError("DigestiveSystemController requires both food prefab references.", this);
        if (digestivePlatform == null)
            Debug.LogError("DigestiveSystemController requires the Digestive System platform reference for Vomit.", this);

        RefreshButtonStates();
    }

    private void OnDestroy()
    {
        if (dayCounter != null)
            dayCounter.OnTimeChanged -= HandleTimeChanged;
        if (eatButton != null)
            eatButton.onClick.RemoveListener(HandleEatPressed);
        if (vomitButton != null)
            vomitButton.onClick.RemoveListener(HandleVomitPressed);
    }

    private void Update()
    {
        RefreshButtonStates();
    }

    private void HandleTimeChanged(int hour, int minute)
    {
        if (minute != 0 || (hour != 8 && hour != 12 && hour != 18))
            return;

        int currentDay = dayCounter != null ? dayCounter.CurrentDay : 0;
        if (lastScheduledDay == currentDay && lastScheduledHour == hour)
            return;

        lastScheduledDay = currentDay;
        lastScheduledHour = hour;
        scheduledEatLockUntil = Mathf.Max(scheduledEatLockUntil, Time.time + ScheduledEatLockSeconds);
        StartBatch(UnityEngine.Random.value < ContaminatedBatchChance ? contaminatedFoodPrefab : foodPrefab);
    }

    private void HandleEatPressed()
    {
        if (eatButton == null || !eatButton.interactable || isBatchRunning)
            return;

        eatCooldownUntil = Time.time + ManualEatCooldownSeconds;
        StartBatch(UnityEngine.Random.value < ContaminatedBatchChance ? contaminatedFoodPrefab : foodPrefab);
        RefreshButtonStates();
    }

    private void HandleVomitPressed()
    {
        if (vomitButton == null || !vomitButton.interactable)
            return;

        vomitCooldownUntil = Time.time + VomitCooldownSeconds;
        DestroyFoodOnDigestiveSystem();
        RefreshButtonStates();
    }

    private void StartBatch(GameObject prefab)
    {
        if (prefab == null || mouthPoint == null || rectumPoint == null)
            return;

        if (activeBatch != null)
            StopCoroutine(activeBatch);

        isBatchRunning = false;
        activeBatch = StartCoroutine(SpawnBatchRoutine(prefab));
        RefreshButtonStates();
    }

    private IEnumerator SpawnBatchRoutine(GameObject prefab)
    {
        isBatchRunning = true;
        for (int index = 0; index < FoodPerBatch; index++)
        {
            SpawnFood(prefab);
            if (index < FoodPerBatch - 1)
                yield return new WaitForSeconds(SpawnIntervalSeconds);
        }

        isBatchRunning = false;
        activeBatch = null;
        RefreshButtonStates();
    }

    private void SpawnFood(GameObject prefab)
    {
        NavMeshAgent prefabAgent = prefab.GetComponent<NavMeshAgent>();
        if (prefabAgent == null)
        {
            Debug.LogError($"Food prefab '{prefab.name}' requires a NavMeshAgent.", prefab);
            return;
        }

        NavMeshQueryFilter filter = new NavMeshQueryFilter
        {
            agentTypeID = prefabAgent.agentTypeID,
            areaMask = NavMesh.AllAreas
        };

        if (!NavMesh.SamplePosition(mouthPoint.position, out NavMeshHit spawnHit, NavMeshSampleRadius, filter))
        {
            Debug.LogWarning("No Digestive System NavMesh position was found near Mouth.", this);
            return;
        }

        if (!NavMesh.SamplePosition(rectumPoint.position, out NavMeshHit destinationHit, NavMeshSampleRadius, filter))
        {
            Debug.LogWarning("No Digestive System NavMesh position was found near Rectum.", this);
            return;
        }

        GameObject food = Instantiate(prefab, spawnHit.position, mouthPoint.rotation, transform);
        NavMeshAgent agent = food.GetComponent<NavMeshAgent>();
        if (agent == null || (!agent.isOnNavMesh && !agent.Warp(spawnHit.position)))
        {
            Debug.LogWarning($"Spawned food '{food.name}' could not attach to the Digestive System NavMesh.", food);
            Destroy(food);
            return;
        }

        if (!agent.SetDestination(destinationHit.position))
        {
            Debug.LogWarning($"Spawned food '{food.name}' could not set its Rectum destination.", food);
            Destroy(food);
            return;
        }

        DigestiveFoodAgent foodAgent = food.GetComponent<DigestiveFoodAgent>();
        if (foodAgent == null)
            foodAgent = food.AddComponent<DigestiveFoodAgent>();
        foodAgent.Initialize(agent, ArrivalDistance);
    }

    private void DestroyFoodOnDigestiveSystem()
    {
        if (digestivePlatform == null)
            return;

        Renderer[] platformRenderers = digestivePlatform.GetComponentsInChildren<Renderer>(true);
        if (platformRenderers.Length == 0)
        {
            Debug.LogWarning("Vomit could not find rendered bounds for the Digestive System platform.", digestivePlatform);
            return;
        }

        Bounds platformBounds = platformRenderers[0].bounds;
        for (int index = 1; index < platformRenderers.Length; index++)
            platformBounds.Encapsulate(platformRenderers[index].bounds);
        platformBounds.Expand(DigestiveBoundsPadding);

        Transform[] sceneTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform candidate in sceneTransforms)
        {
            if (candidate == null || candidate.gameObject.scene != gameObject.scene || !IsFoodPrefabInstance(candidate))
                continue;

            if (platformBounds.Contains(candidate.position))
                Destroy(candidate.gameObject);
        }
    }

    private bool IsFoodPrefabInstance(Transform candidate)
    {
        if (candidate.GetComponent<DigestiveFoodAgent>() != null)
            return true;

        string objectName = candidate.name;
        return HasPrefabName(objectName, foodPrefab) || HasPrefabName(objectName, contaminatedFoodPrefab);
    }

    private static bool HasPrefabName(string objectName, GameObject prefab)
    {
        if (prefab == null)
            return false;

        return objectName == prefab.name ||
               objectName == prefab.name + "(Clone)" ||
               objectName.StartsWith(prefab.name + " (", StringComparison.Ordinal);
    }

    private void RefreshButtonStates()
    {
        if (eatButton != null)
        {
            float remainingCooldown = Mathf.Max(eatCooldownUntil, scheduledEatLockUntil) - Time.time;
            bool isOnCooldown = remainingCooldown > 0f;
            eatButton.interactable = !isBatchRunning && !isOnCooldown;
            if (eatLabel != null)
            {
                eatLabel.text = isOnCooldown
                    ? $"EAT ({Mathf.CeilToInt(remainingCooldown)}s)"
                    : isBatchRunning ? "EATING..." : "EAT";
            }
        }

        if (vomitButton != null)
        {
            float remainingCooldown = vomitCooldownUntil - Time.time;
            vomitButton.interactable = remainingCooldown <= 0f;
            if (vomitLabel != null)
                vomitLabel.text = remainingCooldown > 0f ? $"VOMIT ({Mathf.CeilToInt(remainingCooldown)}s)" : "VOMIT";
        }
    }
}
