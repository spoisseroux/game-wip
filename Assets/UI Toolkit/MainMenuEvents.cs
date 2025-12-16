using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    private UIDocument _document;
    private Button _startButton;
    private Button _continueButton;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _startButton = _document.rootVisualElement.Q("ButtonPlay") as Button;
        _continueButton = _document.rootVisualElement.Q("ButtonContinue") as Button;

        _startButton.RegisterCallback<ClickEvent>(OnPlayGameClick);
        _continueButton.RegisterCallback<ClickEvent>(OnContinueGameClick);
    }

    private void OnPlayGameClick(ClickEvent evt)
    {
        Debug.Log("Play game clicked");
    }

    private void OnContinueGameClick(ClickEvent evt)
    {
        Debug.Log("Continue game clicked");
    }

    private void OnDisable()
    {
        _startButton.UnregisterCallback<ClickEvent>(OnPlayGameClick);
        _continueButton.UnregisterCallback<ClickEvent>(OnContinueGameClick);
    }
}
