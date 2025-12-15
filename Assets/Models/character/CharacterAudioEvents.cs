using FMODUnity;
using UnityEngine;

public class CharacterAudioEvents : MonoBehaviour
{
    public StudioEventEmitter footstepEmitter;
    public StudioEventEmitter jumpEmitter;
    public StudioEventEmitter attackEmitter;

    // Animation Events call THESE EXACT names

    public void Footstep()
    {
        footstepEmitter.Play();
    }

    public void Jump()
    {
        jumpEmitter.Play();
    }

    public void Attack()
    {
        attackEmitter.Play();
    }
}
