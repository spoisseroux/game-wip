using UnityEngine;

public class PersistentParentObject : MonoBehaviour
{
    public static PersistentParentObject instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}