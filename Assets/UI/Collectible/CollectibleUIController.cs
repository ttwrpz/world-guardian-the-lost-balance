using UnityEngine;
using UnityEngine.UIElements;

public class CollectibleUIController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    private Button _backButton;
    private Button _endingButton;
    private Button _itemButton;
    private Button _landmarkButton;
    private Button _loreButton;
    private Label _endingLabel;
    private Label _itemLabel;
    private Label _landmarkLabel;
    private Label _loreLabel;

    private CollectibleManager _collectibleManager;

    private void OnEnable()
    {
        _collectibleManager = FindFirstObjectByType<CollectibleManager>();
        if (_collectibleManager == null)
        {
            Debug.LogError("CollectibleManager not found in the scene.");
            return;
        }

        GetUIElements();
        AttachEventHandlers();
    }

    private void GetUIElements()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _endingButton = _root.Q<Button>("EndingButton");
        _endingLabel = _root.Q<Label>("EndingLabel");
        UpdateCollectibleLabel(_endingLabel, Collectible.CollectibleType.Endings);

        _itemButton = _root.Q<Button>("ItemButton");
        _itemLabel = _root.Q<Label>("ItemLabel");
        UpdateCollectibleLabel(_itemLabel, Collectible.CollectibleType.Items);

        _landmarkButton = _root.Q<Button>("LandmarkButton");
        _landmarkLabel = _root.Q<Label>("LandmarkLabel");
        UpdateCollectibleLabel(_landmarkLabel, Collectible.CollectibleType.Landmarks);

        _loreButton = _root.Q<Button>("LoreButton");
        _loreLabel = _root.Q<Label>("LoreLabel");
        UpdateCollectibleLabel(_loreLabel, Collectible.CollectibleType.Lores);

        _backButton = _root.Q<Button>("BackButton");
    }

    private void UpdateCollectibleLabel(Label label, Collectible.CollectibleType type)
    {
        int collectedCount = _collectibleManager.GetCollectedCountByType(type);
        int totalCount = _collectibleManager.GetTotalCountByType(type);

        label.text = label.text
            .Replace("%s1", collectedCount.ToString())
            .Replace("%s2", totalCount.ToString());
    }

    private void AttachEventHandlers()
    {
        _endingButton.clicked += CollectibleUIEventHandlers.onEndingButtonClicked;
        _itemButton.clicked += CollectibleUIEventHandlers.onItemButtonClicked;
        _landmarkButton.clicked += CollectibleUIEventHandlers.onLandmarkButtonClicked;
        _loreButton.clicked += CollectibleUIEventHandlers.onLoreButtonClicked;
        _backButton.clicked += UIManager.BackToMainMenuUI;
    }
}