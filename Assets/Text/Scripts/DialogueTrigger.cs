using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueConversation conversation;
    private DialogueManager dialogueManager;
    private bool playerInside = false;
    private Canvas interactIcon;
    private Quaternion originalRotation;
    private Transform camTransform;
    private GameObject interactIconGO;

    //TODO: Update to use raycast and not triggerbox
    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        //get interact icon
        interactIconGO = GameObject.FindGameObjectWithTag("InteractIcon");
        interactIconGO.SetActive(false);
        interactIcon = interactIconGO.GetComponent<Canvas>();

        //set up camera for transforming interact icon
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        camTransform = mainCamera.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            interactIconGO.SetActive(true);

            //TODO: hardcoded vertical offset for interact icon
            interactIcon.transform.position = transform.position + Vector3.up * 1f;
            Debug.Log("Player entered trigger zone");

            originalRotation = transform.rotation;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            interactIconGO.SetActive(false);

            Debug.Log("Player left trigger zone");
        }
    }

    void Update()
    {
        if (playerInside &&
            !dialogueManager.conversationStarted &&
            !dialogueManager.waitingForRelease)
        {
            //billboard interacticon effect
            interactIcon.transform.rotation = camTransform.rotation * originalRotation;
            //TODO: hardcoded input
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log("E pressed: starting conversation");
                dialogueManager.StartConversation(conversation, this.transform);
            }
        }
    }
}