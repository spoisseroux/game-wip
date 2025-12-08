using System;
using UnityEngine;

public abstract class SaveableObject : MonoBehaviour
{
    protected ISaveData data;
    public string guid;

    private void AssignID()
    {
        guid = Guid.NewGuid().ToString();
    }
}
