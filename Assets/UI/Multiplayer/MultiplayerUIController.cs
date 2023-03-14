using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MultiplayerUIController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    private TextField _serverIpInput;
    private Button _joinServerButton;
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

        _serverIpInput = _root.Q<TextField>("serverIpInput");
        
        _joinServerButton = _root.Q<Button>("joinServerButton");
        _joinServerButton.SetEnabled(false);

        _backButton = _root.Q<Button>("backButton");
    }
    private void AttachEventHandlers()
    {
        _serverIpInput.RegisterCallback<ChangeEvent<string>>(onServerIpInputChanged);
        _serverIpInput.Focus();

        _joinServerButton.clicked += onJoinServerButtonClicked;
        _backButton.clicked += onBackButtonClicked;
    }

    private void onServerIpInputChanged(ChangeEvent<string> changeEvent)
    {
        if (changeEvent.newValue.ToString() == "")
        {
            _joinServerButton.SetEnabled(false);
            _joinServerButton.AddToClassList("btn-disabled");
        }
        else
        {
            _joinServerButton.SetEnabled(true);
            _joinServerButton.RemoveFromClassList("btn-disabled");
        }
    }

    private void onJoinServerButtonClicked()
    {
        if (_joinServerButton.ClassListContains("btn-disabled"))
            return;

        try
        {
            // TODO: Implement Join Server method
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred while trying to join the server: {ex.Message}");
        }
    }

    private async void onBackButtonClicked()
    {
        try
        {
            await SceneManager.LoadSceneAsync("Assets/Scenes/Main/Main.unity");
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred while trying to load the gameplay scene: {ex.Message}");
        }
    }
}
