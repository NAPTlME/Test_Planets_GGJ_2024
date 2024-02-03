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
    private CinemachineBrain mainCamBrain;
    public Camera miniCamera;
    private CinemachineBrain miniCamBrain;

    public CameraType MainCameraType;

    public event Action<CameraType> OnMainCameraChanged;

    public float minOrbitalFov = 40;
    public float maxOrbitalFov = 80;
    public float minOverheadFov = 3;
    public float maxOverheadFov = 10;
    public float zoomSpeedOverlay = 1f;
    public float zoomSpeedOrbital = 5f;

    // Start is called before the first frame update
    void Start()
    {
        MainCameraType = CameraType.Overhead;
        mainCamBrain = mainCamera.GetComponent<CinemachineBrain>();
        miniCamBrain = miniCamera.GetComponent<CinemachineBrain>();
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
        if (MainCameraType.Equals(CameraType.Overhead))
        {
            // switching overhead camera to mini render. Set zoom out to max
            SetFov(maxOverheadFov);
        }
        MainCameraType = MainCameraType == CameraType.Orbital ? CameraType.Overhead : CameraType.Orbital;
        // manually setting the bitmask by string values here
        var overheadMask = LayerMask.GetMask(new string[] { "Default", "TransparentFx", "Ignore Raycast", "Water", "UI", "OverheadCamera", "OrbitalPlanet", "LocalPlanet", "Entity" });
        var orbitalMask = LayerMask.GetMask(new string[] { "Default", "TransparentFx", "Ignore Raycast", "Water", "UI", "PlanetCamera", "OrbitalPlanet", "LocalPlanet", "Entity" });
        mainCamera.cullingMask = MainCameraType == CameraType.Overhead ? overheadMask : orbitalMask;
        miniCamera.cullingMask = MainCameraType == CameraType.Overhead ? orbitalMask : overheadMask;
        OnMainCameraChanged?.Invoke(MainCameraType);
    }
    private float GetCurrentFov()
    {
        var fov = MainCameraType == CameraType.Overhead ? ((CinemachineVirtualCamera)mainCamBrain.ActiveVirtualCamera).m_Lens.FieldOfView : ((CinemachineFreeLook)mainCamBrain.ActiveVirtualCamera).m_Lens.FieldOfView;
        return fov;
    }
    private void SetFov(float x)
    {
        if (MainCameraType == CameraType.Overhead)
        {
            ((CinemachineVirtualCamera)mainCamBrain.ActiveVirtualCamera).m_Lens.FieldOfView = x;
        }
        else
        {
            ((CinemachineFreeLook)mainCamBrain.ActiveVirtualCamera).m_Lens.FieldOfView = x;
        }
    }
    public void ZoomMainCam(float mouseScrollYDelta)
    {
        var currFov = GetCurrentFov();
        var minFov = MainCameraType == CameraType.Overhead ? minOverheadFov : minOrbitalFov;
        var maxFov = MainCameraType == CameraType.Overhead ? maxOverheadFov : maxOrbitalFov;
        float zoomSpeed = MainCameraType == CameraType.Overhead ? zoomSpeedOverlay : zoomSpeedOrbital;

        currFov -= mouseScrollYDelta * zoomSpeed;

        currFov = Mathf.Clamp(currFov, minFov, maxFov);

        SetFov(currFov);
    }
    public PlanetGravity GetOrbitalTarget()
    {
        var brain = MainCameraType.Equals(CameraType.Orbital) ? mainCamBrain : miniCamBrain;
        if (brain.IsBlending)
        {
            return ((CinemachineFreeLook)brain.ActiveBlend.CamB).transform.parent.GetComponentInChildren<PlanetGravity>();
        }
        return ((CinemachineFreeLook)brain.ActiveVirtualCamera).transform.parent.GetComponentInChildren<PlanetGravity>();
    }
}
