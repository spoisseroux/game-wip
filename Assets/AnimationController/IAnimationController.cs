public interface IAnimationController
{
    public void Play(string animName);
    public void CrossFade(string animName, float crossfade);
    public void ForceToAnim(string animName);
    public void SetDefaultAnim(string defA);
    public void ToDefaultAnim();
}