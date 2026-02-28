using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

public class RuneCarousel
{
    public List<UIRuneContainer> runeCarousel;
    private List<RuneDataSO> availableRunes;

    public void Initialize(List<RuneDataSO> runes, GameObject prefabIn)
    {
        availableRunes = runes;

        foreach (var rune in availableRunes)
        {
            GameObject.Instantiate(prefabIn); // position
        }
    }

    public int Size => availableRunes.Count;
}

public class UIRuneContainer : MonoBehaviour
{
    public GameObject runeContainerPrefab;
    public RuneDataSO runeData;
    public Image sprite;

    public void Initialize(RuneDataSO runeInput)
    {
        runeData = runeInput;
    }
}

/*
    How do I want to enable this?
    Event from...
        1. UI Manager?
        2. Player Manager?
        3. errrm secret third thing?

*/
public class RuneUIManager : MonoBehaviour
{
    // owned objects
    /*
        1. think in terms of main frame
        2. children, and their relevant functions
        3. etc.
        
        Then draw out the way you'd like it to work in the hierarchy and create around that
    */
    [Header("UI References")]
    public GameObject mainPanel; // for aspecting, position etc?? is this even needed lel, seems like runeUIManager could handle this
    public GameObject selectionPanel; // for chant/exit
    public RuneCarousel carouselContainer; // make a generic UI Carousel component?? :3 with a template?? :3
    public List<UIRuneContainer> runeCarousel; // maybe this has the little UI containers too
    public List<UIRuneContainer> runeSelection; // maybe an object for the little UI containers housing a rune
    public GameObject runePrefab;

    // input link
    [Header("Input References")]
    [SerializeField] InputReader input;

    // stored data
    private List<RuneDataSO> availableRunes;
    private List<RuneDataSO> chant;
    private int carouselIndex;

    // events for input and chant
    public event Action<List<RuneDataSO>> OnChantSelected;
    public event Action OnClose;

    // move coroutine
    private Coroutine moveCoroutine = null;

    #region MonoBehaviour
    private void Awake()
    {
        carouselIndex = 0;
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        // disable player, maybe this call happens somewhere else

        carouselIndex = 0;

        // populate carousel

        // set all components activate

        // link input
        LinkToInputEvents();
    }

    private void OnDisable()
    {
        // detach input
        DisconnectFromInputEvents();
    }
    #endregion

    #region Event Link
    private void LinkToInputEvents()
    {
        input.EnableActionMapByName("UI");

        // hook up events
        input.OnSelectInput += AttemptSelect;
        input.OnExitInput += AttemptExit;
        input.OnMoveUpInput += AttemptUpMove;
        input.OnMoveDownInput += AttemptDownMove;
        input.OnMoveLeftInput += AttemptLeftMove;
        input.OnMoveRightInput += AttemptRightMove;

        // mouse input is public Vector2 input.mouseInput; do we even need?
    }

    private void DisconnectFromInputEvents()
    {
        input.OnSelectInput -= AttemptSelect;
        input.OnExitInput -= AttemptExit;
        input.OnMoveUpInput -= AttemptUpMove;
        input.OnMoveDownInput -= AttemptDownMove;
        input.OnMoveLeftInput -= AttemptLeftMove;
        input.OnMoveRightInput -= AttemptRightMove;

        input.DisableActionMapByName("UI");
    }
    #endregion

    #region UI Requests 
    private void AttemptSelect()
    {
        // if moving or not hovering, don't allow
        if (moveCoroutine != null)
            return;

        // check whether on carousel or options

            // add to chant

            // exit/chant 
    }

    private void AttemptExit()
    {
        if (moveCoroutine != null)
            return;

        // anything other than close, and invoke disable event?
        OnClose?.Invoke();

        // disable
        this.gameObject.SetActive(false);
    }


    // for all of these, just change index based on.... yeah
    // left/right for menus like runeUI, idk think on it baby
    private void AttemptUpMove()
    {
        
    }

    private void AttemptDownMove()
    {
        
    }

    private void AttemptLeftMove()
    {
        if (moveCoroutine != null)
            return;

        if (carouselIndex == 0)
            return;

        carouselIndex--;
        ShiftMenu(carouselIndex);
    }

    private void AttemptRightMove()
    {
        if (moveCoroutine != null)
            return;

        // no rotating carousel, block right move if at highest index
        if (carouselIndex == runeCarousel.Count - 1)
            return;

        carouselIndex++;
        ShiftMenu(carouselIndex);
    }

    #endregion

    #region Helpers
    public void InitializeMenu(List<RuneDataSO> runes)
    {
        availableRunes = runes;


        // set up carousel
        foreach (var rune in availableRunes)
        {
            Instantiate(runePrefab);
            // runePrefab.Initialize(rune);
        }
        // carouselContainer.Initialize(availableRunes);
    }

    public void CleanUpMenu()
    {
        availableRunes = null;
        chant = null;
        this.gameObject.SetActive(false);
    }

    private void ShiftMenu(int index)
    {
        // physically move the carousel to the chosen index
    }

    // add rune to chant
    private void SelectRune(int index)
    {
        
    }

    // remove the latest rune added to the chant
    private void DeselectRune()
    {
        if (chant.Count == 0)
            return;
        
        chant.RemoveAt(chant.Count - 1);
    }

    #endregion
}