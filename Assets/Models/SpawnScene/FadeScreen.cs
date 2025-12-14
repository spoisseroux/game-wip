using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FadeScreen : MonoBehaviour
{
    public static FadeScreen instance;
    public static bool FadeCompleted;
    public float speed = 1;

    public Material Screen;
    //public AudioMixer Audio;

    private float FadeTime
    {
        set
        {
            Screen.SetFloat("_FadeTime", value);
            //Audio.SetFloat("_MasterVolume", Mathf.Lerp(0, -80, value));
        }
    }

    private void Awake()
    {
        instance = this;
        FadeIn();
    }

    public void FadeIn() => StartCoroutine(interpolate(1, 0));
    public void FadeOut() => StartCoroutine(interpolate(0, 1));

    private IEnumerator interpolate(float from, float to)
    {
        FadeCompleted = false;
        float cur = from;
        for (float t = 0; cur != to; t += Time.deltaTime * speed)
        {
            cur = Mathf.Clamp01(Mathf.SmoothStep(from, to, t));
            FadeTime = cur;
            yield return null;
        }
        FadeCompleted = true;
    }
}