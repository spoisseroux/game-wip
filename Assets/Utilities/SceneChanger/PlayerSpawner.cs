using UnityEditor.SearchService;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (SceneTransitionManager.playerSpawnData != null) 
        {
            // get and adjust player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = SceneTransitionManager.playerSpawnData.position;
            player.transform.rotation = SceneTransitionManager.playerSpawnData.rotation;

            // reset after reading
            SceneTransitionManager.ResetSpawnTransform();
        }
    }
}
