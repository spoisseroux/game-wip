using UnityEngine;
public class SpawnableLocation : MonoBehaviour
{
    [SerializeField] protected Transform spawn;
    [SerializeField] protected int spawnID; // some semantics tbd here, is it total # of doors, scene segregated # of doors? yeah

    public int GetSpawnID()
    {
        return spawnID;
    }

    public Transform GetSpawnTransform()
    {
        return spawn;
    }
}
