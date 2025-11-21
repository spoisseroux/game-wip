using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public interface IInteractable
{
    public abstract void Interact(PlayerMovementManager player); // maybe should be an IEnumerator???
    public bool IsTrigger();
    public void FreePlayer();
}


/*
public interface IInteractable<T> : IInteractable
    {
        T GetContext();

        void SetContext(T context);
    }


    T ==> context, i.e. weapondataSO, itemdataSO, etc.
*/