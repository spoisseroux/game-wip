using UnityEngine;

public class DoorFadeTargetManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The material with the StarfieldDoor shader (will create instance automatically)")]
    public Material starfieldMaterial;

    [Tooltip("First GameObject to track (usually the player)")]
    public Transform targetObject1;

    [Tooltip("Second GameObject to track (optional, usually the camera)")]
    public Transform targetObject2;

    [Tooltip("Extra distance to add to fade calculation (increase this to ensure both player and camera see through)")]
    public float fadeOffset = 2.0f;

    [Tooltip("Update the shader every frame")]
    public bool updateEveryFrame = true;

    private static readonly int FadeTargetID = Shader.PropertyToID("_FadeTarget");
    private static readonly int FadeOffsetID = Shader.PropertyToID("_FadeOffset");

    private Material instanceMaterial;
    private Renderer meshRenderer;

    void Start()
    {
        if (starfieldMaterial == null)
        {
            Debug.LogError("DoorFadeTargetManager: No material assigned!");
            return;
        }

        // Get the renderer on this GameObject
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("DoorFadeTargetManager: No Renderer found on this GameObject!");
            return;
        }

        // Create a unique material instance for this door
        instanceMaterial = new Material(starfieldMaterial);
        meshRenderer.material = instanceMaterial;

        UpdateShaderTarget();
    }

    void Update()
    {
        if (updateEveryFrame && instanceMaterial != null)
        {
            UpdateShaderTarget();
        }
    }

    void UpdateShaderTarget()
    {
        if (instanceMaterial == null) return;

        Vector3 targetPosition = Vector3.zero;
        bool hasTarget = false;

        if (targetObject1 != null && targetObject2 != null)
        {
            // Use midpoint between player and camera
            targetPosition = (targetObject1.position + targetObject2.position) * 0.5f;
            hasTarget = true;
        }
        else if (targetObject1 != null)
        {
            // Only target 1 exists
            targetPosition = targetObject1.position;
            hasTarget = true;
        }
        else if (targetObject2 != null)
        {
            // Only target 2 exists
            targetPosition = targetObject2.position;
            hasTarget = true;
        }

        if (hasTarget)
        {
            // Send position and offset to shader
            instanceMaterial.SetVector(FadeTargetID, new Vector4(targetPosition.x, targetPosition.y, targetPosition.z, 1.0f));
            instanceMaterial.SetFloat(FadeOffsetID, fadeOffset);
        }
        else
        {
            // No targets - use camera position
            instanceMaterial.SetVector(FadeTargetID, new Vector4(0, 0, 0, 0));
            instanceMaterial.SetFloat(FadeOffsetID, fadeOffset);
        }
    }

    void OnValidate()
    {
        // Update in editor when values change
        if (instanceMaterial != null)
        {
            UpdateShaderTarget();
        }
    }

    void OnDestroy()
    {
        // Clean up the material instance
        if (instanceMaterial != null)
        {
            Destroy(instanceMaterial);
        }
    }
}