using UnityEngine.UIElements;
using static Collectible;

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

    public void SetData(Collectible collectible)
    {
        _collectibleWrapper.AddToClassList(collectible.isCollected ? "CollectibleUnlocked" : "CollectibleLocked");
        _icon.style.backgroundImage = new StyleBackground(collectible.icon);
        _collectibleNameLabel.text = collectible.collectibleName;
        _collectibleDescriptionLabel.text = !collectible.isCollected && (collectible.type == CollectibleType.Endings || collectible.type == CollectibleType.Lores)
            ? "???"
            : collectible.description;
        _collectibleStatusLabel.text = (collectible.isCollected) ? "Collected on " + collectible.collectedDate : "Not Collected Yet";
    }
}
