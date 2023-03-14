using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private void OnEnable()
    {
        GetUIElements();
        AttachEventHandlers();
    }

    private void GetUIElements()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _endingButton = _root.Q<Button>("EndingButton");
        _endingLabel = _root.Q<Label>("EndingLabel");
        _endingLabel.text = _endingLabel.text
            .Replace("%s1", "")
            .Replace("%s2", "");

        _itemButton = _root.Q<Button>("ItemButton");
        _itemLabel = _root.Q<Label>("ItemLabel");
        _itemLabel.text = _itemLabel.text
            .Replace("%s1", "")
            .Replace("%s2", "");

        _landmarkButton = _root.Q<Button>("LandmarkButton");
        _landmarkLabel = _root.Q<Label>("LandmarkLabel");
        _landmarkLabel.text = _landmarkLabel.text
            .Replace("%s1", "")
            .Replace("%s2", "");

        _loreButton = _root.Q<Button>("LoreButton");
        _loreLabel = _root.Q<Label>("LoreLabel");
        _loreLabel.text = _loreLabel.text
            .Replace("%s1", "")
            .Replace("%s2", "");

        _backButton = _root.Q<Button>("BackButton");

    }

    private void AttachEventHandlers()
    {
        _endingButton.clicked += CollectibleUIEventHandlers.onEndingButtonClicked;
        _itemButton.clicked += CollectibleUIEventHandlers.onItemButtonClicked;
        _landmarkButton.clicked += CollectibleUIEventHandlers.onLandmarkButtonClicked;
        _loreButton.clicked += CollectibleUIEventHandlers.onLoreButtonClicked;
        _backButton.clicked += UIController.BackToMainUI;
    }
}