using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public class ScriptableObjectDictionary<TKey, TValue> : IEnumerable
{
    [SerializeField]
    private List<TKey> keys = new();

    [SerializeReference]
    private List<TValue> values = new();

    public ScriptableObjectDictionary(params (TKey, TValue)[] Inputs)
    {
        foreach ((TKey, TValue) input in Inputs)
        {
            Add(input.Item1, input.Item2);
        }
    }

    public List<TKey> GetGeys()
    {
        return keys;
    }
    public List<TValue> GetValues()
    {
        return values;
    }

    public void Add(TKey key, TValue value)
    {
        if(keys.Contains(key))
        {
            Debug.LogWarning($"Cant add element, key already exists");
            return;
        }

        keys.Add(key);
        values.Add(value);
    }

    public void Remove(TKey key)
    {
        if (!keys.Contains(key))
        {
            Debug.LogWarning($"Cant remove value,missing key inside dictionary");
            return;
        }

        int index = keys.IndexOf(key);
        values.RemoveAt(index);
        keys.Remove(key);
    }

    public void RemoveAt(int index)
    {
        if (index >= 0 && index < keys.Count && index < values.Count)
        {
            values.RemoveAt(index);
            keys.RemoveAt(index);
        }
        else
        {
            Debug.LogWarning($"Cant remove element, index less or bigger then size of array");
        }
    }

    public TValue GetValueAt(int index)
    {
        if (index > values.Count - 1)
        {
            Debug.LogWarning($"Cant get value, index bigger then array size");
            return default;
        }
        return values[index];
    }

    public TKey GetKeyAt(int index)
    {
        if(index > keys.Count - 1)
        {
            Debug.LogWarning($"Cant get key, index bigger then array size");
            return default;
        }
        return keys[index];
    }

    public void Clear()
    {
        keys.Clear();
        values.Clear();
    }

    public void ReplaceKey(TValue value, TKey key)
    {
        if(values.Contains(value))
        {
            keys[values.IndexOf(value)] = key;
            return;
        }
        Debug.LogWarning($"Missing value for replacement");

    }

    public bool ContainsKey(TKey key)
    {
        return keys.Contains(key);
    }

    public bool ContainsValue(TValue value)
    {
        return values.Contains(value);
    }

    public int Count()
    {
        return keys.Count;
    }

    //indexer
    public TValue this[TKey key]
    {
        get
        {
            if (ContainsKey(key))
            {
                return values[keys.IndexOf(key)];
            }
            else
            {
                Debug.LogError("Missing Key inside the Dictionary");
                return default;
            }
        }
        set
        {
            if (ContainsKey(key))
            {
                values[keys.IndexOf(key)] = value;
            }
            else
            {
                keys.Add(key);
                values.Add(value);
            }
        }
    }

    public KeyValuePair<TKey,TValue> GetAtIndex(int index)
    {
        KeyValuePair<TKey, TValue> returnv = new(GetKeyAt(index),GetValueAt(index));
        return returnv;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            yield return new KeyValuePair<TKey, TValue>(keys[i], values[i]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}