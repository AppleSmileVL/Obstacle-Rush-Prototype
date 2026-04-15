using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float width = 0.2f;
    [SerializeField] private Material material;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>() ?? gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = lineRenderer.endWidth = width;
        lineRenderer.loop = false;

        if (material != null)
        {
            lineRenderer.material = material;
        }
        else
        {
            var shader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Texture") ?? Shader.Find("Unlit/Color");
            if (shader != null)
                lineRenderer.material = new Material(shader) { hideFlags = HideFlags.DontSave };
        }
    }

    private void Update()
    {
        if (lineRenderer == null || startPoint == null || endPoint == null) return;
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }

    private void OnValidate()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null) return;
        lineRenderer.startWidth = lineRenderer.endWidth = width;
    }
}
