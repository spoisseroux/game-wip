using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueConversation conversation;
    private DialogueManager dialogueManager;
    private bool playerInside = false;

    //TODO: Update to use raycast and not triggerbox
    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Player entered trigger zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("Player left trigger zone");
        }
    }

    void Update()
    {
        if (playerInside &&
            !dialogueManager.conversationStarted &&
            !dialogueManager.waitingForRelease)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log("E pressed: starting conversation");
                dialogueManager.StartConversation(conversation, this.transform);
            }
        }
    }
}