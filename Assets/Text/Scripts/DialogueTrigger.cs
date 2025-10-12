using UnityEngine;

// IDEA:
// If the object inheriting from IInteractable has the word Trigger in its class name, 
// then it triggers an Interaction that changes State in the PlayerFSM
public class DialogueTrigger : MonoBehaviour, IInteractable
{
    public DialogueConversation conversation;
    [SerializeField] private DialogueManager dialogueManager;
    private bool playerInside = false;
    private Canvas interactIcon;
    private Quaternion originalRotation;
    [SerializeField] private Transform camTransform;
    private GameObject interactIconGO;

    private bool playerInteracting = false;

    #region Monobehavior
    void Start()
    {
        //get interact icon
        interactIconGO = GameObject.FindGameObjectWithTag("InteractIcon");
        interactIconGO.SetActive(false);
        interactIcon = interactIconGO.GetComponent<Canvas>();
    }

    void Update()
    {
        //rotate interact icon while player inside
        if (playerInside)
        {
            interactIcon.transform.rotation = camTransform.rotation * originalRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            interactIconGO.SetActive(true);

            //TODO: hardcoded vertical offset for interact icon (place icon above char)
            interactIcon.transform.position = transform.position + Vector3.up * 1f;
            originalRotation = transform.rotation;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            interactIconGO.SetActive(false);
        }
    }
    #endregion

    #region IInteractable Interface Methods
    public void Interact()
    {
        // set to busy
        playerInteracting = true;
        // start convo
        if (!(dialogueManager.conversationStarted && dialogueManager.waitingForRelease))
            dialogueManager.StartConversation(conversation, this.transform);
        // enable UI action map from here?

    }

    public bool IsTrigger()
    {
        return true;
    }

    public void FreePlayer()
    {
        // end convo
        // set to free
        playerInteracting = false;
        // enable player Action map from here?
    }
    #endregion
}