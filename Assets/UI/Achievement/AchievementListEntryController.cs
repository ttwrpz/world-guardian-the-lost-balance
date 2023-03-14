using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AchievementListEntryController
{
    GroupBox _achievementWrapper;
    VisualElement _icon;
    Label _achievementNameLabel;
    Label _achievementDescriptionLabel;
    Label _achievementStatusLabel;

    public void SetVisualElement(VisualElement visualElement)
    {
        _achievementWrapper = visualElement.Q<GroupBox>("AchievementWrapper");
        _icon = visualElement.Q<VisualElement>("Icon");
        _achievementNameLabel = visualElement.Q<Label>("AchievementNameLabel");
        _achievementDescriptionLabel = visualElement.Q<Label>("AchievementDescriptionLabel");
        _achievementStatusLabel = visualElement.Q<Label>("AchievementStatusLabel");
    }

    public void SetData(Achievement achievement)
    {
        _achievementWrapper.AddToClassList((achievement.IsUnlocked) ? "AchievementUnlocked" : "AchievementLocked");
        _icon.style.backgroundImage = new StyleBackground(achievement.Icon);
        _achievementNameLabel.text = achievement.Name;
        _achievementDescriptionLabel.text = achievement.Description;
        _achievementStatusLabel.text = (achievement.IsUnlocked) ? "Unlocked on " + achievement.UnlockedDate : "Not Unlocked Yet";
    }
}
