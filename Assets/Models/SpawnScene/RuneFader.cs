using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneFader : MonoBehaviour
{
    public static RuneFader instance;
    public static bool FadeCompleted;

    [Header("Fade Settings")]
    public float speed = 1;

    [Header("Materials to Fade")]
    public Material echoMaterial;      // Your SpriteEchoShader material
    public Material scrollMaterial;    // Optional: Your HorizontalFadeScrollShader material

    [Header("GameObject Management")]
    [Tooltip("Disable this GameObject after fade out completes")]
    public bool disableGameObjectAfterFadeOut = false;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        instance = this;

        // Cache reference
        meshRenderer = GetComponent<MeshRenderer>();

        FadeIn();
    }

    private void SetMaterialOpacity(float value)
    {
        // Set opacity on materials if they exist
        if (echoMaterial != null)
        {
            echoMaterial.SetFloat("_OverallOpacity", value);
        }
        if (scrollMaterial != null)
        {
            scrollMaterial.SetFloat("_OverallOpacity", value);
        }
    }

    public void FadeIn()
    {
        // Re-enable this GameObject if it was disabled
        if (disableGameObjectAfterFadeOut && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        // Re-enable mesh renderer if it was disabled
        if (meshRenderer != null && !meshRenderer.enabled)
        {
            meshRenderer.enabled = true;
        }

        StartCoroutine(Interpolate(0, 1));
    }

    public void FadeOut() => StartCoroutine(Interpolate(1, 0));

    private IEnumerator Interpolate(float from, float to)
    {
        FadeCompleted = false;
        float cur = from;

        for (float t = 0; cur != to; t += Time.deltaTime * speed)
        {
            cur = Mathf.Clamp01(Mathf.SmoothStep(from, to, t));
            SetMaterialOpacity(cur);
            yield return null;
        }

        // Ensure we reach the final value
        SetMaterialOpacity(to);
        FadeCompleted = true;

        // Handle fade out completion
        if (to == 0)
        {
            HandleFadeOutComplete();
        }
    }

    private void HandleFadeOutComplete()
    {
        // ALWAYS disable mesh renderer after fade out
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        // Reset material opacity to 1 so it can be reused elsewhere
        ResetMaterialOpacity();

        // Optionally disable this GameObject (happens last)
        if (disableGameObjectAfterFadeOut)
        {
            gameObject.SetActive(false);
        }
    }

    private void ResetMaterialOpacity()
    {
        // Reset all materials to full opacity for reuse
        if (echoMaterial != null)
        {
            echoMaterial.SetFloat("_OverallOpacity", 1f);
        }
        if (scrollMaterial != null)
        {
            scrollMaterial.SetFloat("_OverallOpacity", 1f);
        }
    }

    // Additional utility methods
    public void FadeTo(float targetOpacity, float customSpeed = -1)
    {
        float useSpeed = customSpeed > 0 ? customSpeed : speed;
        float currentOpacity = GetCurrentOpacity();
        StartCoroutine(FadeToCoroutine(currentOpacity, targetOpacity, useSpeed));
    }

    private IEnumerator FadeToCoroutine(float from, float to, float fadeSpeed)
    {
        FadeCompleted = false;
        float cur = from;

        for (float t = 0; Mathf.Abs(cur - to) > 0.001f; t += Time.deltaTime * fadeSpeed)
        {
            cur = Mathf.SmoothStep(from, to, t);
            SetMaterialOpacity(cur);
            yield return null;
        }

        SetMaterialOpacity(to);
        FadeCompleted = true;

        // Handle fade out completion if fading to 0
        if (to == 0)
        {
            HandleFadeOutComplete();
        }
    }

    // Get current opacity
    public float GetCurrentOpacity()
    {
        if (echoMaterial != null)
        {
            return echoMaterial.GetFloat("_OverallOpacity");
        }
        else if (scrollMaterial != null)
        {
            return scrollMaterial.GetFloat("_OverallOpacity");
        }
        return 0;
    }

    // Immediate set without fade
    public void SetOpacityImmediate(float opacity)
    {
        SetMaterialOpacity(Mathf.Clamp01(opacity));

        // Handle fade out behavior for immediate opacity changes
        if (opacity == 0)
        {
            HandleFadeOutComplete();
        }
    }

    // Manual cleanup method (useful if you want to trigger the behavior without fading)
    public void ForceCleanup()
    {
        HandleFadeOutComplete();
    }

    // Re-enable everything (useful for reusing the same setup)
    public void ResetAndEnable()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }

        SetMaterialOpacity(1f);
    }
}