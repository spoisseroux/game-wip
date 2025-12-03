using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
    Reserved for things like Doors which specifically react to a sequence of runes, and would generate an interaction as a result
*/
public interface IChantReactor
{
    public abstract void React(List<RuneType> runes);
}

public class RuneDoor : MonoBehaviour, IChantReactor
{
    public Material before;
    public Material after;

    [SerializeField]
    public List<RuneType> runeCode;

    #region MonoBehaviour
    private void Awake()
    {
        GetComponent<Renderer>().material = before;
    }


    #endregion

    #region IChantReactor
    public void React(List<RuneType> runes)
    {
        if (runeCode.Count != runes.Count) 
            return;

        for (int i = 0; i < runeCode.Count; i++)
        {
            if (runes[i] != runeCode[i])
                return;
        }

        OpenRoutine();
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
