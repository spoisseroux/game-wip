using UnityEngine;
using System.Collections.Generic;

public class RuneDoor : MonoBehaviour, IChantReactor
{
    public Material before;
    public Material after;

    [SerializeField]
    public List<RuneType> code;

    #region MonoBehaviour
    private void Awake()
    {
        GetComponent<Renderer>().material = before;
    }
    #endregion

    #region IChantReactor
    public void React(List<RuneType> runes)
    {
        Debug.Log("Woah someone wanted something from me the humble RoonDoar...");
        if (IsValidChant(runes))
        {
            OpenRoutine();
        }
    }
    #endregion

    #region Helpers
    public bool IsValidChant(List<RuneType> runes)
    {
        if (code.Count != runes.Count) 
            return false;

        for (int i = 0; i < code.Count; i++)
        {
            if (runes[i] != code[i])
                return false;
        }

        return true;
    }
    #endregion

    public void OpenRoutine()
    {
        // make noises
        // make shader fx
        GetComponent<Renderer>().material = after;
        // open doors
    }
}