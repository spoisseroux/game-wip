using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class SerializeableQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public SerializeableQuaternion(Quaternion q)
    {
        this.x = q.x;
        this.y = q.y;
        this.z = q.z;
        this.w = q.w;
    }

    [JsonIgnore]
    public Quaternion UnityQuaternion
    {
        get
        {
            return new Quaternion(this.x, this.y, this.z, this.w);
        }
    }

    public static List<SerializeableQuaternion> GetSerializeableQuaternionList(List<Quaternion> qList)
    {
        List<SerializeableQuaternion> sQ = new List<SerializeableQuaternion>();
        for (int i = 0; i < qList.Count; i++)
        {
            sQ.Add(new SerializeableQuaternion(qList[i]));
        }

        return sQ;
    }

    public List<Quaternion> GetQuaternionList(List<SerializeableQuaternion> sqList)
    {
        List<Quaternion> q = new List<Quaternion>();
        for (int i = 0; i < sqList.Count; i++)
        {
            q.Add(sqList[i].UnityQuaternion);
        }

        return q;
    }
}
