using UnityEngine.UIElements;

public class WorldListEntryController
{
    Label _worldNameLabel;
    Label _worldFolderLabel;
    Label _worldGameModeLabel;
    Label _worldDifficultyLabel;
    Label _worldSizeLabel;
    Label _worldCreatedAtLabel;
    Label _worldModifiedAtLabel;

    public void SetVisualElement(VisualElement visualElement)
    {
        _worldNameLabel = visualElement.Q<Label>("worldNameLabel");
        _worldFolderLabel = visualElement.Q<Label>("worldFolderLabel");
        _worldGameModeLabel = visualElement.Q<Label>("worldGameModeLabel");
        _worldDifficultyLabel = visualElement.Q<Label>("worldDifficultyLabel");
        _worldCreatedAtLabel = visualElement.Q<Label>("worldCreatedAtLabel");
        _worldModifiedAtLabel = visualElement.Q<Label>("worldModifiedAtLabel");
    }

    public void SetData(World world)
    {
        _worldNameLabel.text = world.WorldName;
        _worldFolderLabel.text = string.Concat("Saved at ", world.WorldFolder);
        _worldGameModeLabel.text = world.WorldGameMode.ToString();
        _worldDifficultyLabel.text = world.WorldDifficulty.ToString();
        _worldCreatedAtLabel.text = _worldCreatedAtLabel.text.Replace("%s", world.WorldModifiedAt.ToBinary() == 0 ? "Unknown" : world.WorldCreatedAt.ToString());
        _worldModifiedAtLabel.text = _worldModifiedAtLabel.text.Replace("%s", world.WorldModifiedAt.ToBinary() == 0 ? "Unknown" : world.WorldModifiedAt.ToString());
    }
}
