using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera launchCamera;
    public CinemachineVirtualCamera planetCamera;

    public OrbitRig planetCameraRig;


    private CinemachineVirtualCamera currentCamera;

    // Start is called before the first frame update
    void Start()
    {
        currentCamera = planetCamera;
        currentCamera.Priority = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCamera(CinemachineVirtualCamera newFocus)
    {
        currentCamera.Priority = 0;
        newFocus.Priority = 1;
    }

    public void SetFocusTarget(Transform target)
    {
        planetCameraRig.target = target;
    }
}
