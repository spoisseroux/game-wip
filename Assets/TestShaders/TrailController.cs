using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TrailRenderer))]
public class TrailController : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Material shader;

    [Header("Configs")]
    [SerializeField] bool active;
    [SerializeField] float lingerTime;
    [SerializeField] Vector3 displacementMax;

    private float _currentAlpha = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        trailRenderer.startWidth = 0.5f;
        trailRenderer.endWidth = 0.0f;
        trailRenderer.time = lingerTime;
        trailRenderer.material = shader;

        // Smoothly drive the material alpha so it doesn't pop
        float target = active ? 1f : 0f;
        float speed  = 0.1f;
        _currentAlpha = Mathf.MoveTowards(_currentAlpha, target, speed * Time.deltaTime);

        trailRenderer.emitting = active;

        Color sc = trailRenderer.startColor; sc.a = _currentAlpha; trailRenderer.startColor = sc;
        Color ec = trailRenderer.endColor;   ec.a = 0f;             trailRenderer.endColor   = ec;
    }
}
