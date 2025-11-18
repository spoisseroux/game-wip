using Unity.VisualScripting;
using UnityEngine;

public interface IInteractable
{
    public abstract void Interact();
    public virtual bool IsTrigger() { return false; }
    public virtual void FreePlayer() { }
}


/*
public interface IInteractable<T> : IInteractable
    {
        T GetContext();

        void SetContext(T context);
    }


    T ==> context, i.e. weapondataSO, itemdataSO, etc.
*/