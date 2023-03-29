using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public abstract class UIController : MonoBehaviour
{
    protected UIDocument _doc;
    protected VisualElement _root;

    protected virtual void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        GetUIElements();
        AttachEventHandlers();
    }

    protected abstract void GetUIElements();

    protected abstract void AttachEventHandlers();

}