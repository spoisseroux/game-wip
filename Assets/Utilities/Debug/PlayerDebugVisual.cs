using TMPro;
using UnityEngine;

public class PlayerDebugVisual : MonoBehaviour
{
    [SerializeField] PlayerMovementManager player;
    [SerializeField] TextMeshProUGUI textBox;

    void Awake()
    {
        
    }

    void Update()
    {
        textBox.text = player.GetState();
    }
}
