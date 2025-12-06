using UnityEngine;

public class EyeBlink : MonoBehaviour
{
    [Header("Blink Settings")]
    [Tooltip("Average seconds between blinks")]
    [Range(1f, 10f)]
    public float blinkFrequency = 4f;

    [Tooltip("Randomness factor (0 = no randomness, 1 = max randomness)")]
    [Range(0f, 1f)]
    public float randomness = 0.4f;

    [Header("Blink Animation")]
    [Tooltip("How long the blink takes in seconds")]
    [Range(0.05f, 0.3f)]
    public float blinkDuration = 0.15f;

    [Tooltip("Curve for natural blink motion (starts at 1=open, dips to 0=closed, returns to 1=open)")]
    public AnimationCurve blinkCurve;

    [Header("Material Method")]
    [Tooltip("The renderer with the eye material")]
    public Renderer eyeRenderer;

    [Tooltip("Material index if using multiple materials")]
    public int materialIndex = 0;

    private Material eyeMaterial;
    private float nextBlinkTime;
    private bool isBlinking = false;
    private float blinkTimer = 0f;

    void Awake()
    {
        // Initialize curve with proper default if it's not set
        if (blinkCurve == null || blinkCurve.keys.Length == 0)
        {
            blinkCurve = new AnimationCurve(
                new Keyframe(0f, 1f),      // Start: eye open (scale = 1)
                new Keyframe(0.5f, 0.05f), // Middle: eye closed (scale = 0.05)
                new Keyframe(1f, 1f)       // End: eye open (scale = 1)
            );
            // Smooth the curve
            for (int i = 0; i < blinkCurve.keys.Length; i++)
            {
                blinkCurve.SmoothTangents(i, 0);
            }
        }
    }

    void Start()
    {
        // Get the material instance
        if (eyeRenderer == null)
            eyeRenderer = GetComponent<Renderer>();

        if (eyeRenderer != null)
        {
            eyeMaterial = eyeRenderer.materials[materialIndex];
        }
        else
        {
            Debug.LogError("No Renderer found for eye blink!");
            enabled = false;
            return;
        }

        ScheduleNextBlink();
    }

    void Update()
    {
        if (isBlinking)
        {
            PerformBlink();
        }
        else if (Time.time >= nextBlinkTime)
        {
            StartBlink();
        }
    }

    void StartBlink()
    {
        isBlinking = true;
        blinkTimer = 0f;
    }

    void PerformBlink()
    {
        blinkTimer += Time.deltaTime;
        float progress = blinkTimer / blinkDuration;

        if (progress >= 1f)
        {
            // Blink finished - reset texture scale
            eyeMaterial.SetTextureScale("_BaseMap", new Vector2(1f, 1f));
            eyeMaterial.SetTextureOffset("_BaseMap", new Vector2(0f, 0f));
            isBlinking = false;
            ScheduleNextBlink();
        }
        else
        {
            // Animate the blink using the curve
            // Curve goes from 1 (open) to 0.05 (closed) and back to 1
            float curveValue = blinkCurve.Evaluate(progress);

            // INVERT: smaller texture scale = texture stretched (eye appears squashed)
            // So we want HIGH scale values when blinking (curve is low)
            float scaleY = 1f / Mathf.Max(curveValue, 0.05f);

            // Scale texture vertically and offset to keep it centered
            eyeMaterial.SetTextureScale("_BaseMap", new Vector2(1f, scaleY));
            eyeMaterial.SetTextureOffset("_BaseMap", new Vector2(0f, (1f - scaleY) * 0.5f));
        }
    }

    void ScheduleNextBlink()
    {
        // Calculate next blink with randomness
        float randomOffset = Random.Range(-randomness, randomness) * blinkFrequency;
        float nextInterval = blinkFrequency + randomOffset;
        nextInterval = Mathf.Max(nextInterval, 0.5f); // Minimum 0.5s between blinks

        nextBlinkTime = Time.time + nextInterval;
    }

    void OnDestroy()
    {
        // Clean up material instance
        if (eyeMaterial != null)
        {
            Destroy(eyeMaterial);
        }
    }
}