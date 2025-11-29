using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour, IAnimationController
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    public string defaultState;

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

    public void SetDefaultAnim(string defaultAnim)
    {
        animator.Play(defaultAnim);
    }

    public void ToDefaultAnim()
    {
        animator.Play(defaultState);
    }
    #endregion
}