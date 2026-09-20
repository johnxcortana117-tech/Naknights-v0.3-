using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIDocument_Script : MonoBehaviour
{
    public UIDocument uiDocument; // assign uidocument
    public InnateCell_Spawner innatecell_spawner; // assign spawn point

    [SerializeField] public LayerMask selectableLayer; // set to selectable
    public Transform _currentSelection;

    private Button macrophage;
    private Button neutrophil;
    private Label day;

    // session
    public int dayCount = 1;
    public float dayTimer = 0f;
    public float dayLimit = 60f;

    // economy
    public int resourceCount = 10000;
    public int macrophageCost = 250;
    public int neutrophilCost = 150;
    public int basophilCost = 500;

    // execution constraint
    public float cooldownDuration = 1f;

    void OnEnable()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;
        VisualElement root = uiDocument.rootVisualElement;

        Button neutrophil = root.Q<Button>("button1");
        Button macrophage = root.Q<Button>("button2");
        Button basophil = root.Q<Button>("button3");
        //Button nk = root.Q<Button>("button4");

        Label day = root.Q<Label>("day");
        Label resource = root.Q<Label>("resource");

        day.text = $"Day {dayCount}";
        //resource.text = resourceCount.ToString();
        resource.text = $"Resource: {resourceCount}";

        if (neutrophil != null)
            neutrophil.clicked +=
                () =>
                {
                    Debug.Log("Before: " + resourceCount);
                    resourceCount = resourceCount - neutrophilCost;
                    //resource.text = resourceCount.ToString();
                    resource.text = $"Resource: {resourceCount}";
                    Debug.Log("After: " + resourceCount);

                    StartCoroutine(NeutrophilRoutine(neutrophil, () => innatecell_spawner.SpawnNeutrophil()));
                };

        if (macrophage != null)
            macrophage.clicked +=
                () =>
                {
                    Debug.Log("Before: " + resourceCount);
                    resourceCount = resourceCount - macrophageCost;
                    //resource.text = resourceCount.ToString();
                    resource.text = resourceCount.ToString();
                    Debug.Log("After: " + resourceCount);

                    StartCoroutine(MacrophageRoutine(macrophage, () => innatecell_spawner.SpawnMacrophage()));
                };

        if (basophil != null) basophil.clicked += () => StartCoroutine(BasophilRoutine(basophil));
    }

    void Update()
    {
        // RESETS GAME
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }

        // GAME DURATION, COUNTS DAYS
        dayTimer += Time.deltaTime;
        if (dayTimer >= dayLimit)
        {
            dayCount++;
            Debug.Log("Day " + dayCount);

            if (day != null)
            {
                day.text = $"Day {dayCount}";
            }
            dayTimer = 0f;
        }

        // PATHOGEN SELECTOR
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            DeselectCurrent();

            Vector2 mousePosition = Mouse.current.position.ReadValue();

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
            {

                Transform selection = hit.transform;
                SelectObject(selection);

            }
        }

        // CHECKS FOR RESOURCE
        if (macrophageCost > resourceCount)
        {
            if (macrophage != null)
            {
                macrophage.SetEnabled(false);
            }
        }
        if (!(neutrophilCost <= resourceCount))
        {
            if (neutrophil != null)
            {
                neutrophil.SetEnabled(false);
            }
        }
    }

    private IEnumerator MacrophageRoutine(Button button, System.Action spawnAction)
    {
        button.SetEnabled(false);
        spawnAction.Invoke();
        yield return new WaitForSeconds(cooldownDuration);
        if (macrophageCost < resourceCount)
        {
            button.SetEnabled(true);
        }
    }

    private IEnumerator NeutrophilRoutine(Button button, System.Action spawnAction)
    {
        button.SetEnabled(false);
        spawnAction.Invoke();
        yield return new WaitForSeconds(cooldownDuration);
        if (neutrophilCost < resourceCount)
        {
            button.SetEnabled(true);
        }
    }
    private IEnumerator BasophilRoutine(Button button)
    {
        button.SetEnabled(false);

        cooldownDuration = 0.5f;
        yield return new WaitForSeconds(15f);

        cooldownDuration = 1f;
        yield return new WaitForSeconds(15f);

        if (macrophageCost < resourceCount)
        {
            button.SetEnabled(true);
        }
    }

    private void SelectObject(Transform selection)
    {

        _currentSelection = selection;

        var selectionRenderer = _currentSelection.GetComponent<Renderer>();
        if (selectionRenderer != null)
        {
            selectionRenderer.material.color = Color.darkRed; // or new material
        }
        Debug.Log(_currentSelection.name);
    }

    public void DeselectCurrent()
    {
        if (_currentSelection != null)
        {
            var selectionRenderer = _currentSelection.GetComponent<Renderer>();
            if (selectionRenderer != null)
            {
                selectionRenderer.material.color = Color.gray; // or original material
            }

            _currentSelection = null;
        }
    }
}