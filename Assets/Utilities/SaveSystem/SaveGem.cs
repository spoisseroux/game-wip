using UnityEngine;
using UnityEngine.Playables;

// test object designed specifically to save the game upon player entering this object
public class SaveGem : MonoBehaviour, IHittable
{
    [SerializeField] PlayableDirector director;

    #region MonoBehaviour
    // Enter this collider to save
    void OnTriggerEnter(Collider other)
    {
        /*
        if (other.CompareTag("Player"))
        {
            SaveGameManager yeah = GameObject.Find("SaveManager").GetComponent<SaveGameManager>();
            yeah.SaveGame();
        }
        */
    }
    #endregion

    // hitbox struck this collider with a Hitbox, perform a save.... HAVE TO DO SOURCE RESOLUTION HERE TOO SO NPCs CANNOT SAVE FOR US LOL
    #region IHittable Interface
    public void Hit(HitboxRecord hit)
    {
        // play
        director.Play();

        SaveGameManager yeah = GameObject.Find("SaveManager").GetComponent<SaveGameManager>();
        yeah.SaveGame();
    }
    #endregion

}
