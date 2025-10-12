using Unity.VisualScripting;
using UnityEngine;

public interface IInteractable
{
    public abstract void Interact();
    public virtual bool IsTrigger() { return false; }
    public virtual void FreePlayer() { }
}
