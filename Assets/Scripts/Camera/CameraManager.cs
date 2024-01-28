using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CinemachineBrain miniCamera;

    public CinemachineVirtualCamera mainLaunchCamera;
    public CinemachineVirtualCamera mainPlanetCamera;

    public CinemachineVirtualCamera miniLaunchCamera;
    public CinemachineVirtualCamera miniPlanetCamera;

    public OrbitRig planetCameraRig;
    public SunCameraRig launchRig;

    private CinemachineVirtualCamera currentMainCamera;
    private CinemachineVirtualCamera currentMiniCamera;

    // Start is called before the first frame update
    void Start()
    {
        currentMainCamera = mainLaunchCamera;
        currentMiniCamera = miniLaunchCamera;
        currentMainCamera.Priority = 1;
        currentMiniCamera.Priority = 1;

        launchRig.isFocused = true;
        planetCameraRig.isFocused = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwapCameras()
    {
        if(launchRig.isFocused)
        {
            launchRig.isFocused = false;
            planetCameraRig.isFocused = true;

            SetCamera(mainPlanetCamera);
            SetMiniCamera(miniLaunchCamera);
        } else
        {
            launchRig.isFocused = true;
            planetCameraRig.isFocused = false;

            SetCamera(mainLaunchCamera);
            SetMiniCamera(miniPlanetCamera);
        }
    }

    public void SetCamera(CinemachineVirtualCamera newFocus)
    {
        currentMainCamera.Priority = 0;
        newFocus.Priority = 1;

        currentMainCamera = newFocus;

        if(newFocus.Equals(mainLaunchCamera))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else if (newFocus.Equals(mainPlanetCamera))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SetMiniCamera(CinemachineVirtualCamera newFocus)
    {
        currentMiniCamera.Priority = 0;
        newFocus.Priority = 1;

        currentMiniCamera = newFocus;
    }

    public void SetFocusTarget(Transform target)
    {
        planetCameraRig.target = target;
    }
}
