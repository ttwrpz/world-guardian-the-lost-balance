using UnityEngine;
using UnityEngine.UIElements;

public class AchievementUIController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    private ListView _achievementList;
    private Button _backButton;

    private void Awake()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;
    }
}
