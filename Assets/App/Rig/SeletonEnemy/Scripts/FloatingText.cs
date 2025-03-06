using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float floatSpeed = 1.5f;
    public float lifetime = 1f;

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy after time
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime; // Move up
    }

    public void SetText(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
    }
}
