using UnityEngine;
using System.Collections.Generic;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class RuneDatabase : MonoBehaviour
{
    [SerializeField] public RuneDataSO[] runesList;

    private Dictionary<int, RuneDataSO> database;
    private static RuneDatabase instance;

    #region MonoBehaviour
    private void Awake()
    {
        instance = this;
        database = new();

        // add all runes
        foreach (RuneDataSO rune in runesList)
        {
            database.Add(rune.databaseID, rune);
        }
    }
    #endregion

    public static RuneDataSO GetRune(int id)
    {
        return instance.database[id];
    }

#if UNITY_EDITOR
    [ContextMenu("Autofill DB")]
    private void Autofill()
    {
        var guids = AssetDatabase.FindAssets("t:RuneDataSO");
        List<RuneDataSO> items = new List<RuneDataSO>();

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var rune = AssetDatabase.LoadAssetAtPath<RuneDataSO>(path);
            items.Add(rune);
        }

        runesList = items.ToArray();
    }
#endif
}