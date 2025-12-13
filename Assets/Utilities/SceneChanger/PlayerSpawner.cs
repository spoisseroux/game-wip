using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (SceneTransitionManager.playerSpawnData != null) 
        {
            // adjust player
            player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = SceneTransitionManager.playerSpawnData.position;
            player.transform.rotation = SceneTransitionManager.playerSpawnData.rotation;

            // reset after reading
            SceneTransitionManager.ResetSpawnTransform();
        }
        else
        {
            player.transform.position = SceneTransitionManager.startPosition;
            player.transform.rotation = SceneTransitionManager.startRotation;
        }
    }
}
