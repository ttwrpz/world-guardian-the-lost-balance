using UnityEngine;
using UnityEngine.UIElements;

public class CreditUIController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    private Button _backButton;

    private void OnEnable()
    {
        GetUIElements();
        AttachEventHandlers();
    }

    private void GetUIElements()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _backButton = _root.Q<Button>("BackButton");
        _backButton.clicked += UIManager.BackToMainMenuUI;
    }

    private void AttachEventHandlers()
    {
        _backButton.clicked += UIManager.BackToMainMenuUI;
    }
}
