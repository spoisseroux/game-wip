using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GlyphDatabase : MonoBehaviour
{
    // singleton
    private static GlyphDatabase instance;

    [SerializeField] public GlyphDataSO[] glyphList;

    private Dictionary<int, GlyphDataSO> database;

    #region MonoBehaviour
    private void Awake()
    {
        // singleton call
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        InitializeDB();
    }
    #endregion

    private void InitializeDB()
    {
        // add all runes
        foreach (GlyphDataSO glyph in glyphList)
        {
            // check for existence
            database.TryAdd(glyph.databaseID, glyph);
        }
    }

    public static GlyphDataSO GetRune(int id)
    {
        return instance.database[id];
    }

#if UNITY_EDITOR
    [ContextMenu("Autofill DB")]
    private void Autofill()
    {
        var guids = AssetDatabase.FindAssets("t:RuneDataSO");
        List<GlyphDataSO> items = new List<GlyphDataSO>();

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var glyph = AssetDatabase.LoadAssetAtPath<GlyphDataSO>(path);
            items.Add(glyph);
        }

        glyphList = items.ToArray();
    }
#endif
}