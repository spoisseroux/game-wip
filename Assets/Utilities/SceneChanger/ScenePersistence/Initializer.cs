using UnityEngine;

/*
    A script gathered online in order to instantiate a mass of singleton systems all at once
    Used because it plays nicely with both Build and Editor contexts
*/ 

/*
public static class Initializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    public static void Execute()
    {
        Debug.Log("Loaded by Persistent Objects from Initializer Script");
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("PERSISTOBJECTS")));
    }
}
*/
