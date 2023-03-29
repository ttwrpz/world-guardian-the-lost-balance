using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlyCameraExtended : MonoBehaviour
{
    FlyCamera flyCamera;

    void Start()
    {
        flyCamera = GetComponent<FlyCamera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(flyCamera.initPositionButton))
        {
            ToggleFlyCameraScript();
        }
    }

    void ToggleFlyCameraScript()
    {
        if (!flyCamera.enabled) flyCamera.enabled = true;

        flyCamera.SetInitPosition();
        flyCamera._active = !flyCamera._active;
        
        if (flyCamera._active)
        {
            flyCamera._wantedMode = CursorLockMode.Locked;
        }
        else
        {
            flyCamera._wantedMode = CursorLockMode.None;
        }
            

        Cursor.lockState = flyCamera._wantedMode;
        Cursor.visible = (CursorLockMode.Locked != flyCamera._wantedMode);
    }
}
