using UnityEngine;
using UnityEngine.AI;

public class BacteriaScript_A : MonoBehaviour
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
        else if (target != null)
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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Tissue A");
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
            if (hit.CompareTag("Tissue A"))
            {
                TissueHealthScript tissue = hit.GetComponent<TissueHealthScript>();
                if (tissue.tissueHP > 0) {
                    tissue.TakeDamage(damage);
                }
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

/*
RESERVE CODES

[SerializeField] public LayerMask selectableLayer;
public Transform _currentSelection;

void Update(){
    if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            DeselectCurrent();

            Vector2 mousePosition = Mouse.current.position.ReadValue();

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
            {
                //Debug.Log("Mouse click detected by Input System!");
                Transform selection = hit.transform;
                SelectObject(selection);
            }
        }
}

private void SelectObject(Transform selection)
    {
        
        _currentSelection = selection;

        var selectionRenderer = _currentSelection.GetComponent<Renderer>();
        if (selectionRenderer != null)
        {
            selectionRenderer.material.color = Color.darkRed;
        }

        //Debug.Log($"Selected: {_currentSelection.name}");
    }

    public void DeselectCurrent()
    {
        if (_currentSelection != null)
        {
            var selectionRenderer = _currentSelection.GetComponent<Renderer>();
            if (selectionRenderer != null)
            {
                selectionRenderer.material.color = Color.gray;
            }

            _currentSelection = null;
        }
    }

*/