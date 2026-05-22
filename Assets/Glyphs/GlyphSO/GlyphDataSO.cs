using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGlyph", menuName = "GlyphData/Glyph")]
public class GlyphDataSO : ScriptableObject {
    // base data
    public int databaseID;
    public string glyphName;

    // UI
    public Sprite uiImage;

    // audio
    public string audioFileName; // or audio clip or byte[]
    // public AudioClip baseAudio;
    public float pitch;
    public float resonance;
    // etc, etc. research fmod

    // visual
    UnityEngine.VFX.VisualEffectAsset visualEffect;
    Shader shader;
    Texture2D texture;
    Color glyphColor;
    Material material;
    
    // buff system
}