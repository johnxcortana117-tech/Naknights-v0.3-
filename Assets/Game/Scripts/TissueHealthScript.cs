using UnityEngine;

public class TissueHealthScript : MonoBehaviour
{
    public Material material;

    public float tissueHP = 1000f;
    public float gradient;

    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        float gradient = tissueHP / 1000f;

        Color startColor = new Color32(0, 0, 0, 150);
        Color endColor = new Color32(120, 60, 60, 150);

        material.color = Color.Lerp(startColor, endColor, gradient);
    }

    public void TakeDamage(float amount)
    {
        tissueHP -= amount;

        if (tissueHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}