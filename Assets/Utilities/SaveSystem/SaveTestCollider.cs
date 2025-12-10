using UnityEngine;

// test object designed specifically to save the game upon player entering this object
public class SaveTestCollider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveGameManager yeah = GameObject.Find("SaveManager").GetComponent<SaveGameManager>();
            yeah.SaveGame();
        }
    }
}
