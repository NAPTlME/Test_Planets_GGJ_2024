using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum CameraType
{
    Overhead,
    Orbital
}

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;
    public Camera miniCamera;

    public CameraType MainCameraType;

    public event Action<CameraType> OnMainCameraChanged;

    // Start is called before the first frame update
    void Start()
    {
        MainCameraType = CameraType.Overhead;
        /*currentMainCamera = mainLaunchCamera;
        currentMiniCamera = miniPlanetCamera;
        currentMainCamera.Priority = 1;
        currentMiniCamera.Priority = 1;

        launchRig.isFocused = true;
        planetCameraRig.isFocused = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;*/
    }

    public void SwapCameras()
    {
        MainCameraType = MainCameraType == CameraType.Orbital ? CameraType.Overhead : CameraType.Orbital;
        // manually setting the bitmask by string values here
        var overheadMask = LayerMask.GetMask(new string[] { "Default", "TransparentFx", "Ignore Raycast", "Water", "UI", "OverheadCamera", "OrbitalPlanet", "LocalPlanet", "Entity" });
        var orbitalMask = LayerMask.GetMask(new string[] { "Default", "TransparentFx", "Ignore Raycast", "Water", "UI", "PlanetCamera", "OrbitalPlanet", "LocalPlanet", "Entity" });
        mainCamera.cullingMask = MainCameraType == CameraType.Overhead ? overheadMask : orbitalMask;
        miniCamera.cullingMask = MainCameraType == CameraType.Overhead ? orbitalMask : overheadMask;
        OnMainCameraChanged?.Invoke(MainCameraType);
    }
}
