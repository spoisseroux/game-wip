using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour, IAnimationController
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    public string defaultAnim;

    #region MonoBehaviours
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    #endregion

    #region IAnimationController Interface
    public void Play(string animName)
    {
        animator.Play(animName);
    }

    public void CrossFade(string animName, float crossfade)
    {
        animator.CrossFade(animName, crossfade);
    }

    public void ForceToAnim(string animName)
    {
        animator.Play(animName);
    }

    public void SetDefaultAnim(string defA)
    {
        defaultAnim = defA;
    }

    public void ToDefaultAnim()
    {
        animator.Play(defaultAnim);
    }
    #endregion
}