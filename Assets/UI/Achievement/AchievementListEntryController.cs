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
        _achievementWrapper.AddToClassList((achievement.isUnlocked) ? "AchievementUnlocked" : "AchievementLocked");
        _icon.style.backgroundImage = new StyleBackground(achievement.icon);
        _achievementNameLabel.text = achievement.achievementName;
        _achievementDescriptionLabel.text = achievement.description;
        _achievementStatusLabel.text = (achievement.isUnlocked) ? "Unlocked on " + achievement.unlockedDate : "Not Unlocked Yet";
    }
}
