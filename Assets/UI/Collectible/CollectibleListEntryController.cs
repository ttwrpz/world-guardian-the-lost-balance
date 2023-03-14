using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CollectibleListEntryController
{
    GroupBox _collectibleWrapper;
    VisualElement _icon;
    Label _collectibleNameLabel;
    Label _collectibleDescriptionLabel;
    Label _collectibleStatusLabel;

    public void SetVisualElement(VisualElement visualElement)
    {
        _collectibleWrapper = visualElement.Q<GroupBox>("CollectibleWrapper");
        _icon = visualElement.Q<VisualElement>("Icon");
        _collectibleNameLabel = visualElement.Q<Label>("CollectibleNameLabel");
        _collectibleDescriptionLabel = visualElement.Q<Label>("CollectibeDescriptionLabel");
        _collectibleStatusLabel = visualElement.Q<Label>("CollectibleStatusLabel");
    }

    public void SetData(CollectibleSO collectible)
    {
        _collectibleWrapper.AddToClassList((collectible.isUnlocked) ? "CollectibleUnlocked" : "CollectibleLocked");
        _icon.style.backgroundImage = new StyleBackground(collectible.icon);
        _collectibleNameLabel.text = collectible.collectibleName;
        _collectibleDescriptionLabel.text = collectible.description;
        _collectibleStatusLabel.text = (collectible.isUnlocked) ? "Unlocked on " + collectible.unlockedDate : "Not Unlocked Yet";
    }
}
