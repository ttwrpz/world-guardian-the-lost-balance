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
        _worldSizeLabel = visualElement.Q<Label>("worldSizeLabel");
        _worldCreatedAtLabel = visualElement.Q<Label>("worldCreatedAtLabel");
        _worldModifiedAtLabel = visualElement.Q<Label>("worldModifiedAtLabel");
    }

    public void SetWorldData(World world)
    {
        _worldNameLabel.text = world.worldName;
        _worldFolderLabel.text = string.Concat("Saved at ", world.worldFolder);
        _worldGameModeLabel.text = world.worldGameMode.ToString();
        _worldDifficultyLabel.text = world.worldDifficulty.ToString();
        _worldSizeLabel.text = string.Concat(world.worldSize.ToString(), "Size");
        _worldCreatedAtLabel.text += world.worldModifiedAt.ToBinary() == 0 ? "Unknown" : world.worldCreatedAt.ToString();
        _worldModifiedAtLabel.text += world.worldModifiedAt.ToBinary() == 0 ? "Unknown" : world.worldModifiedAt.ToString();
    }
}
