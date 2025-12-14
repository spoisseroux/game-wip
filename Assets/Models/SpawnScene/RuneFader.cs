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
    public Material scrollMaterial;    // Your HorizontalFadeScrollShader material

    //[Header("Optional: Audio")]
    //public UnityEngine.Audio.AudioMixer audioMixer;
    //public bool fadeAudio = false;

    private void Awake()
    {
        instance = this;
        FadeIn();
    }

    private void SetMaterialOpacity(float value)
    {
        // Set opacity on both materials if they exist
        if (echoMaterial != null)
        {
            echoMaterial.SetFloat("_OverallOpacity", value);
        }

        if (scrollMaterial != null)
        {
            scrollMaterial.SetFloat("_OverallOpacity", value);
        }

        // Optional: Fade audio mixer volume
        //if (fadeAudio && audioMixer != null)
        //{
        //   audioMixer.SetFloat("MasterVolume", Mathf.Lerp(-80, 0, value));
        //}
    }

    public void FadeIn() => StartCoroutine(Interpolate(0, 1));

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
    }

    // Additional utility methods
    public void FadeTo(float targetOpacity, float customSpeed = -1)
    {
        float useSpeed = customSpeed > 0 ? customSpeed : speed;
        float currentOpacity = echoMaterial != null ? echoMaterial.GetFloat("_OverallOpacity") : 0;
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
    }
}