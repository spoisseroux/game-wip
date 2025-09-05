using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTest : MonoBehaviour
{
    public DialogueConversation conversation; //Conversation to test
    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (dialogueManager != null && conversation != null)
            {
                dialogueManager.StartConversation(conversation);
            }
        }
    }
}

