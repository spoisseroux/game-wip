using UnityEngine;
using UnityEngine.Playables;

// test object designed specifically to save the game upon player entering this object
public class SaveGem : MonoBehaviour, IHittable
{
    // who can actually hit this object to push a save
    [SerializeField] GameObject saveInitiator;

    // timeline save routine
    [SerializeField] PlayableDirector director;

    // hitbox struck this collider with a Hitbox, perform a save.... HAVE TO DO SOURCE RESOLUTION HERE TOO SO NPCs CANNOT SAVE FOR US LOL
    #region IHittable Interface
    public void Hit(HitboxRecord hit)
    {
        // return if not an object that can hit the save gem
        if (hit.context.source.GetHitboxSourceGameObject() != saveInitiator)
            return;
        
        director.Play();
        
        SaveGameManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveGameManager>();
        saveManager.SaveGame();
    }

    // unique usage of the IHittable interface
    public GameObject GetGameObject()
    {
        return saveInitiator;
    }
    #endregion

}
