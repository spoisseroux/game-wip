using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class SerializeableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializeableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    [JsonIgnore]
    public Vector3 UnityVector
    {
        get
        {
            return new Vector3(x, y, z);
        }
    }

    public static List<SerializeableVector3> GetSerializeableVector3List(List<Vector3> vectors)
    {
        List<SerializeableVector3> convertedList = new List<SerializeableVector3>();
        for (int i = 0; i < vectors.Count; i++)
        {
            convertedList.Add(new SerializeableVector3(vectors[i]));
        }

        return convertedList;
    }

    public static List<Vector3> GetUnityVector3List(List<SerializeableVector3> sVectors)
    {
        List<Vector3> convertedList = new List<Vector3>();
        for (int i = 0; i < sVectors.Count; i++)
        {
            convertedList.Add(sVectors[i].UnityVector);
        }

        return convertedList;
    }
}
